using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Core.Execution.Interrupts;
using Core.Execution.Objects;
using Core.Execution.Objects.BuiltinFunctions;
using Core.Execution.Objects.DataModel;
using Core.Execution.Operations.Binary;
using Core.Execution.Operations.Unary;
using Core.Lexing;
using Core.Syntax.AST;
using Core.Syntax.AST.Expressions;
using Core.Utils.Diagnostic;

namespace Core.Execution
{
    public class Executor : IExecutor
    {
        private readonly BinaryOperation[] binaryOperations;
        private readonly UnaryOperation[] unaryOperations;

        private readonly Stack<ICallable> callStack = new();
        private readonly Stack<Statement> loopStack = new();
        private readonly Scope globalScope = new(null);
        private Scope scope;

        private Obj lastExpressionValue;

        public Executor(
            BinaryOperation[] binaryOperations, 
            UnaryOperation[] unaryOperations, 
            BuiltinFunction[] builtinFunctions)
        {
            this.binaryOperations = binaryOperations;
            this.unaryOperations = unaryOperations;
            
            foreach (var function in builtinFunctions)
                globalScope.Assign(function.Name, function);

            scope = globalScope;
        }

        public TranslationState<Obj> Execute(SyntaxTree tree)
        {
            var diagnostic = new DiagnosticBag();
            lastExpressionValue = new Null();
            
            try
            {
                foreach (var member in tree.Members)
                    member.Accept(this);

                return new TranslationState<Obj>(lastExpressionValue, diagnostic);
            }
            catch (RuntimeException exception)
            {
                diagnostic.AddError(exception.Location, exception.Message);
                
                return new TranslationState<Obj>(null, diagnostic);
            }
        }

        public Obj Execute(FunctionDeclarationStatement statement)
        {
            var positions = statement
                .PositionParameters
                .Select(token => token.Text)
                .ToImmutableArray();

            var defaults = statement
                .DefaultParameters
                .Select(parameter => (parameter.Name.Text, parameter.Expression.Accept(this)))
                .ToImmutableArray();

            var function = new Function(
                statement.Identifier.Text,
                positions,
                defaults,
                context =>
                {
                    var enumerator = statement.Block.Statements.GetEnumerator();
                    while (ReferenceEquals(context.Callable, callStack.Peek()) && enumerator.MoveNext())
                        enumerator.Current.Accept(this);
                },
                isBuiltin: false
            );

            scope.Assign(function.Name, function);

            return null;
        }

        public Obj Execute(ReturnStatement statement)
        {
            if (callStack.Count == 0)
            {
                throw new RuntimeException(
                    statement.Keyword.Location,
                    "The return statement can be only into function block"
                );
            }
                
            var value = statement.Expression?.Accept(this) ?? new Null();
            throw new ReturnInterrupt(value);
        }

        public Obj Execute(BreakStatement statement)
        {
            TryThrowLoopInterrupt(
                new BreakInterrupt(),
                statement.Keyword, 
                "The break statement is only valid inside loop"
            );

            return null;
        }
        
        public Obj Execute(ContinueStatement statement)
        {
            TryThrowLoopInterrupt(
                new ContinueInterrupt(),
                statement.Keyword,
                "The continue statement is only valid inside loop"
            );

            return null;
        }

        public Obj Execute(ForStatement statement)
        {
            loopStack.Push(statement);
            
            foreach (var initializer in statement.Initializers)
                initializer.Accept(this);

            while (statement.Condition.Accept(this).ToBoolean().Value)
            {
                try
                {
                    statement.Body.Accept(this);
                }
                catch (BreakInterrupt)
                {
                    break;
                }
                catch (ContinueInterrupt)
                {
                }
                
                foreach (var iterator in statement.Iterators)
                    iterator.Accept(this);
            }

            loopStack.Pop();
            
            return null;
        }

        public Obj Execute(WhileStatement statement)
        {
            loopStack.Push(statement);

            while (statement.Condition.Accept(this).ToBoolean().Value)
            {
                try
                {
                    statement.Body.Accept(this);
                }
                catch (BreakInterrupt)
                {
                    break;
                }
                catch (ContinueInterrupt)
                {
                }
            }

            loopStack.Pop();
            
            return null;
        }

        public Obj Execute(IfStatement statement)
        {
            if (statement.Condition.Accept(this).ToBoolean().Value)
            {
                statement.ThenStatement.Accept(this);
            }
            else
            {
                statement.ElseClause?.Accept(this);
            }

            return null;
        }

        public Obj Execute(ElseClause clause)
        {
            clause.Statement.Accept(this);
            
            return null;
        }

        public Obj Execute(BlockStatement block)
        {
            var parentScope = scope;
            scope = new Scope(parentScope);
            
            foreach (var statement in block.Statements)
                statement.Accept(this);

            scope = parentScope;

            return null;
        }

        public Obj Execute(ExpressionStatement statement)
        {
            lastExpressionValue = statement.Expression.Accept(this);

            return null;
        }

        public Obj Execute(VariableAssignmentExpression assignment)
        {
            var name = assignment.Identifier.Text;
            var value = assignment.Expression.Accept(this);

            if (!TryAssignUp(name, value))
                scope.Assign(name, value);

            return value;
        }

        public Obj Execute(IndexAssignmentExpression assignment)
        {
            var errorLocation = GetLocationBetweenBrackets(assignment.Index.OpenBracket, assignment.Index.CloseBracket);
            var obj = assignment.IndexedExpression.Accept(this);
            
            if (obj is not IIndexSettable settable)
                throw new RuntimeException(errorLocation, $"Type '{obj.TypeName}' is not settable by index");

            var index = GetIndexValue(assignment.Index);
            var value = assignment.Value.Accept(this);

            try
            {
                return settable[index] = value;
            }
            catch (IndexOutOfRangeException)
            {
                throw new RuntimeException(errorLocation, "The index was outside the bounds of the collection");
            }
        }

        public Obj Execute(ParenthesizedExpression expression) => expression.InnerExpression.Accept(this);

        public Obj Execute(IndexAccessExpression expression)
        {
            var errorLocation = GetLocationBetweenBrackets(expression.Index.OpenBracket, expression.Index.CloseBracket);
            var parent = expression.IndexedExpression.Accept(this);

            if (parent is not IIndexReadable readable)
                throw new RuntimeException(errorLocation, $"Type '{parent.TypeName}' is not readable by index");
            
            try
            {
                var index = GetIndexValue(expression.Index);

                return readable[index];
            }
            catch (IndexOutOfRangeException)
            {
                throw new RuntimeException(errorLocation, "The index was outside the bounds of the collection");
            }
        }

        public Obj Execute(BinaryExpression binary)
        {
            var opToken = binary.OperatorToken;
            
            var left = binary.Left.Accept(this);
            var right = binary.Right.Accept(this);
            var method = binaryOperations
                .Single(op => op.IsOperator(opToken.Type))
                .FindMethod(left, right);

            if (method is not null) 
                return method.Invoke();

            throw new RuntimeException(
                opToken.Location,
                $"The binary operator '{opToken.Text}' is not defined for '{left.TypeName}' and '{right.TypeName}' types"
            );
        }

        public Obj Execute(UnaryExpression unary)
        {
            var opToken = unary.OperatorToken;
            var operand = unary.Operand.Accept(this);
            var method = unaryOperations
                .Single(op => op.IsOperator(opToken.Type))
                .FindMethod(operand);

            if (method is not null) 
                return method.Invoke();

            throw new RuntimeException(
                opToken.Location,
                $"The unary operator '{opToken.Text}' is not defined for '{operand.TypeName}' type"
            );
        }

        public Obj Execute(NumberExpression number) => new Number(number.Value);

        public Obj Execute(BooleanExpression boolean) => new Objects.Boolean(boolean.Value);

        public Obj Execute(StringExpression str) => new Objects.String(str.Value);
        
        public Obj Execute(ArrayExpression array)
        {
            var items = array.Items
                .Select(expression => expression.Accept(this))
                .ToImmutableArray();

            return new Objects.Array(items);
        }

        public Obj Execute(NullExpression expression) => new Null();

        public Obj Execute(VariableExpression variable)
        {
            var identifier = variable.Identifier;
            
            if (scope.TryLookup(identifier.Text, out var value))
                return value;

            throw new RuntimeException(identifier.Location, $"Variable '{identifier.Text}' does not exist");
        }

        public Obj Execute(CallExpression expression)
        {
            var obj = expression.CallableExpression.Accept(this);

            if (obj is not ICallable callable)
            {
                var location = GetLocationBetweenBrackets(expression.OpenParenthesis, expression.CloseParenthesis);

                throw new RuntimeException(location, $"'{obj.TypeName}' object is not callable");
            }
            
            var argumentsCount = expression.Arguments.Length;
            var positionParametersCount = callable.PositionParameters.Length;
            var defaultParametersCount = callable.DefaultParameters.Length;
            
            if (argumentsCount < positionParametersCount 
                || argumentsCount > positionParametersCount + defaultParametersCount)
            {
                throw new RuntimeException(
                    GetLocationBetweenBrackets(expression.OpenParenthesis, expression.CloseParenthesis),
                    $"Callable requires {positionParametersCount} position arguments but was given {argumentsCount}"
                );
            }
            
            var arguments = EvaluateArguments(expression, callable);
            return InvokeCallableObject(callable, arguments);
        }

        private Obj InvokeCallableObject(ICallable callable, ImmutableDictionary<string, Obj> arguments)
        {
            callStack.Push(callable);

            var previousScope = scope;
            scope = new Scope(globalScope);
            
            foreach (var param in arguments)
                scope.Assign(param.Key, param.Value);

            Obj returnedValue = new Null();
            try
            {
                callable.Invoke(new CallContext(callable, scope));
            }
            catch (ReturnInterrupt interrupt)
            {
                returnedValue = interrupt.Value;
            }
            
            scope = previousScope;
            callStack.Pop();

            return returnedValue;
        }

        private ImmutableDictionary<string, Obj> EvaluateArguments(CallExpression expression, ICallable callable)
        {
            var result = ImmutableDictionary.CreateBuilder<string, Obj>();
            
            var arguments = expression.Arguments;
            var positionParameters = callable.PositionParameters;
            var defaultParameters = callable.DefaultParameters;
            
            for (var i = 0; i < positionParameters.Length; i++)
                result[positionParameters[i]] = arguments[i].Accept(this);

            for (var i = 0; i < arguments.Length - positionParameters.Length; i++)
                result[defaultParameters[i].Name] = arguments[i + positionParameters.Length].Accept(this);

            for (var i = 0; i < positionParameters.Length + defaultParameters.Length - arguments.Length; i++)
            {
                var offset = i + arguments.Length - positionParameters.Length;
                result[defaultParameters[offset].Name] = defaultParameters[offset].Value;
            }

            return result.ToImmutable();
        }

        private int GetIndexValue(SyntaxIndex syntaxIndex)
        {
            var errorLocation = GetLocationBetweenBrackets(syntaxIndex.OpenBracket, syntaxIndex.CloseBracket);
            var index = syntaxIndex.Value.Accept(this);
            
            if (index is not Number number)
                throw new RuntimeException(errorLocation, $"Expected number value but was '{index.TypeName}' type");

            if (!number.IsInteger)
                throw new RuntimeException(errorLocation, "Expected integer value");
            
            return (int)number.Value;
        }

        private bool TryAssignUp(string name, Obj value)
        {
            var current = scope;
            do
            {
                if (current.Contains(name))
                {
                    current.Assign(name, value);
                    return true;
                }

                current = current.Parent;
            } while (current is not null);

            return false;
        }
        
        private void TryThrowLoopInterrupt(LoopInterrupt interrupt, SyntaxToken keyword, string errorMessage)
        {
            if (loopStack.Count > 0) 
                throw interrupt;
            
            throw new RuntimeException(keyword.Location, errorMessage);
        }

        private static Location GetLocationBetweenBrackets(SyntaxToken open, SyntaxToken close)
        {
            return new Location(
                open.Location.Line,
                open.Location.Start,
                close.Location.Start - open.Location.Start + 1
            );
        }
    }
}