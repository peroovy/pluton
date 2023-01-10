using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
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
using Core.Utils.Text;

namespace Core.Execution
{
    public class Executor : IExecutor
    {
        private readonly BinaryOperation[] binaryOperations;
        private readonly UnaryOperation[] unaryOperations;

        private readonly Stack<ICallable> callStack = new();
        private readonly Stack<LoopStatement> loopStack = new();
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
            var diagnosticBag = new DiagnosticBag();
            lastExpressionValue = new Null();
            
            try
            {
                foreach (var member in tree.Members)
                    member.Accept(this);

                return new TranslationState<Obj>(lastExpressionValue, diagnosticBag);
            }
            catch (RuntimeException exception)
            {
                diagnosticBag.AddError(exception.Location, exception.Message);
                
                return new TranslationState<Obj>(null, diagnosticBag);
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
                    try
                    {
                        var enumerator = statement.Block.Statements.GetEnumerator();
                        while (ReferenceEquals(context.Callable, callStack.Peek()) && enumerator.MoveNext())
                            enumerator.Current.Accept(this);
                    }
                    catch (ReturnInterrupt interrupt)
                    {
                        return interrupt.Value;
                    }

                    return new Null();
                }
            );

            scope.Assign(function.Name, function);

            return null;
        }

        public Obj Execute(ReturnStatement statement)
        {
            var value = statement.Expression?.Accept(this) ?? new Null();
            
            throw new ReturnInterrupt(value);
        }

        public Obj Execute(BreakStatement statement)
        {
            throw new BreakInterrupt();
        }
        
        public Obj Execute(ContinueStatement statement)
        {
            throw new ContinueInterrupt();
        }

        public Obj Execute(ForStatement statement)
        {
            ExecuteLoopWithPreCondition(
                statement,
                loopStatement =>
                {
                    foreach (var initializer in loopStatement.Initializers)
                        initializer.Accept(this);
                },
                loopStatement =>
                {
                    foreach (var iterator in loopStatement.Iterators)
                        iterator.Accept(this);
                });

            return null;
        }

        public Obj Execute(WhileStatement statement)
        {
            ExecuteLoopWithPreCondition(statement);
            
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
            var indexedExpression = assignment.IndexedExpression;
            var obj = indexedExpression.Accept(this);

            if (obj is not IIndexSettable settable)
                throw new RuntimeException(indexedExpression.Location, $"Type '{obj.TypeName}' is not settable by index");

            var indexValue = (Number)assignment.Index.Accept(this);
            var value = assignment.Value.Accept(this);

            try
            {
                return settable[indexValue.AsInteger] = value;
            }
            catch (IndexOutOfRangeException)
            {
                var betweenBrackets = GetLocationBetweenBrackets(
                    assignment.Index.OpenBracket, assignment.Index.CloseBracket
                );
                throw new RuntimeException(betweenBrackets, "The index was outside the bounds of the collection");
            }
        }

        public Obj Execute(ParenthesizedExpression expression) => expression.InnerExpression.Accept(this);

        public Obj Execute(IndexAccessExpression expression)
        {
            var indexedExpression = expression.IndexedExpression;
            var obj = indexedExpression.Accept(this);

            if (obj is not IIndexReadable readable)
                throw new RuntimeException(indexedExpression.Location, $"Type '{obj.TypeName}' is not readable by index");

            var indexValue = (Number)expression.Index.Accept(this);
            try
            {
                return readable[indexValue.AsInteger];
            }
            catch (IndexOutOfRangeException)
            {
                var betweenBrackets = GetLocationBetweenBrackets(
                    expression.Index.OpenBracket, expression.Index.CloseBracket
                );
                throw new RuntimeException(betweenBrackets, "The index was outside the bounds of the collection");
            }
        }

        public Obj Execute(Index index)
        {
            var expression = index.Expression;
            var value = expression.Accept(this);
            
            if (value is not Number number)
                throw new RuntimeException(expression.Location, $"Expected number value but was '{value.TypeName}' type");

            if (!number.IsInteger)
                throw new RuntimeException(expression.Location, "Expected integer value");
            
            return number;
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

        public Obj Execute(NumberExpression number)
        {
            var value = Convert.ToDouble(number.Token.Text, CultureInfo.InvariantCulture);

            return new Number(value);
        }

        public Obj Execute(BooleanExpression boolean)
        {
            var value = boolean.Token.Type switch
            {
                TokenType.TrueKeyword => true,
                TokenType.FalseKeyword => false,
                _ => throw new ArgumentException($"Bad boolean token {boolean}")
            };

            return new Objects.Boolean(value);
        }

        public Obj Execute(StringExpression str) => new Objects.String(str.Token.Text);
        
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
            var identifier = variable.Token;
            
            if (scope.TryLookup(identifier.Text, out var value))
                return value;

            throw new RuntimeException(identifier.Location, $"Variable '{identifier.Text}' does not exist");
        }

        public Obj Execute(CallExpression expression)
        {
            var callableExpression = expression.CallableExpression;
            var obj = callableExpression.Accept(this);

            if (obj is not ICallable callable)
                throw new RuntimeException(callableExpression.Location, $"'{obj.TypeName}' object is not callable");
            
            var argumentsCount = expression.Arguments.Length;
            var positionParametersCount = callable.PositionParameters.Length;
            var defaultParametersCount = callable.DefaultParameters.Length;
            
            if (argumentsCount < positionParametersCount 
                || argumentsCount > positionParametersCount + defaultParametersCount)
            {
                throw new RuntimeException(
                    GetLocationBetweenBrackets(expression.OpenParenthesis, expression.CloseParenthesis),
                    $"Callable '{callableExpression}' requires {positionParametersCount} position arguments but was given {argumentsCount}"
                );
            }
            
            var arguments = EvaluateArguments(expression, callable);
            return InvokeCallableObject(callable, arguments);
        }

        public Obj Execute(TernaryExpression expression)
        {
            return expression.Condition.Accept(this).ToBoolean().Value
                ? expression.ThenExpression.Accept(this)
                : expression.ElseExpression.Accept(this);
        }

        private Obj InvokeCallableObject(ICallable callable, ImmutableDictionary<string, Obj> arguments)
        {
            callStack.Push(callable);

            var previousScope = scope;
            scope = new Scope(globalScope);
            
            foreach (var param in arguments)
                scope.Assign(param.Key, param.Value);

            var context = new CallContext(callable, scope);
            var returnedValue = callable.Invoke(context);

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

        private void ExecuteLoopWithPreCondition<T>(
            T statement, 
            Action<T> initialize = null, 
            Action<T> updateAfterIteration = null) where T : LoopStatement
        {
            loopStack.Push(statement);

            try
            {
                initialize?.Invoke(statement);

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

                    updateAfterIteration?.Invoke(statement);
                }
            }
            finally
            {
                loopStack.Pop();
            }
        }

        private static Location GetLocationBetweenBrackets(SyntaxToken open, SyntaxToken close)
        {
            return new Location(
                open.Location.SourceText,
                open.Location.Span.Start,
                close.Location.Span.Start - open.Location.Span.Start + 1
            );
        }
    }
}