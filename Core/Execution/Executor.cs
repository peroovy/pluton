using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Core.Diagnostic;
using Core.Execution.Interrupts;
using Core.Execution.Objects;
using Core.Execution.Objects.BuiltinFunctions;
using Core.Execution.Objects.Indexer;
using Core.Execution.Operations.Binary;
using Core.Execution.Operations.Unary;
using Core.Syntax.AST;
using Core.Syntax.AST.Expressions;

namespace Core.Execution
{
    public class Executor : IExecutor
    {
        private readonly BinaryOperation[] binaryOperations;
        private readonly UnaryOperation[] unaryOperations;
        private readonly IDiagnosticBag diagnosticBag;

        private readonly Stack<Function> callStack = new();
        private readonly Stack<Statement> loopStack = new();
        private readonly Scope globalScope = new(null);
        private Scope scope;

        private Obj lastExpressionValue;

        public Executor(
            BinaryOperation[] binaryOperations, 
            UnaryOperation[] unaryOperations, 
            BuiltinFunction[] builtinFunctions, 
            IDiagnosticBag diagnosticBag)
        {
            this.binaryOperations = binaryOperations;
            this.unaryOperations = unaryOperations;
            this.diagnosticBag = diagnosticBag;

            foreach (var function in builtinFunctions)
                globalScope.Assign(function.Name, function);

            scope = globalScope;
        }

        public Obj Execute(SyntaxTree tree)
        {
            foreach (var member in tree.Members)
                member.Accept(this);

            return lastExpressionValue;
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
                statement.Name.Text,
                positions,
                defaults,
                context =>
                {
                    var enumerator = statement.Body.Statements.GetEnumerator();
                    while (ReferenceEquals(context.Function, callStack.Peek()) && enumerator.MoveNext())
                        enumerator.Current.Accept(this);
                },
                isBuiltin: false
            );

            scope.Assign(function.Name, function);

            return null;
        }

        public Obj Execute(ReturnStatement statement)
        {
            if (callStack.Count > 0)
            {
                var value = statement.Expression?.Accept(this) ?? new Null();

                throw new ReturnInterrupt(value);
            }

            var keyword = statement.Keyword;
            diagnosticBag.AddError(keyword.Location, keyword.Length, "The return statement can be only into function block");
            
            throw new RuntimeException();
        }

        public Obj Execute(BreakStatement statement)
        {
            if (loopStack.Count != 0)
                throw new BreakInterrupt();
            
            var keyword = statement.Keyword;
            diagnosticBag.AddError(keyword.Location, keyword.Length, "The break statement is only valid inside loop");

            throw new RuntimeException();
        }
        
        public Obj Execute(ContinueStatement statement)
        {
            if (loopStack.Count != 0) 
                throw new ContinueInterrupt();
            
            var keyword = statement.Keyword;
            diagnosticBag.AddError(keyword.Location, keyword.Length, "The continue statement is only valid inside loop");

            throw new RuntimeException();
        }

        public Obj Execute(ForStatement statement)
        {
            loopStack.Push(statement);
            
            var previousScope = scope;
            scope = new Scope(previousScope);
            
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

            scope = previousScope;

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
                statement.Statement.Accept(this);
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
            var name = assignment.Variable.Text;
            var value = assignment.Expression.Accept(this);

            if (!TryAssignUp(name, value))
                scope.Assign(name, value);

            return value;
        }

        public Obj Execute(IndexAssignmentExpression assignment)
        {
            var openBracket = assignment.Index.OpenBracket;
            var length = assignment.Index.CloseBracket.Location.Position - openBracket.Location.Position + 1;
            
            var obj = assignment.Expression.Accept(this);
            if (obj is not IIndexSettable settable)
            {
                diagnosticBag.AddError(openBracket.Location, length, $"Type '{obj.TypeName}' is not settable by index");

                throw new RuntimeException();
            }

            var index = GetIndex(assignment.Index);
            var value = assignment.Value.Accept(this);

            try
            {
                return settable[index] = value;
            }
            catch (IndexOutOfRangeException)
            {
                diagnosticBag.AddError(openBracket.Location, length, "The index was outside the bounds of the list");

                throw new RuntimeException();
            }
        }

        public Obj Execute(ParenthesizedExpression expression) => expression.InnerExpression.Accept(this);

        public Obj Execute(IndexAccessExpression indexAccess)
        {
            var parent = indexAccess.ParentExpression.Accept(this);

            var openBracket = indexAccess.Index.OpenBracket;
            var closeBracket = indexAccess.Index.CloseBracket;
            var lengthBetween = closeBracket.Location.Position - openBracket.Location.Position + 1;

            if (parent is not IIndexReadable readable)
            {
                diagnosticBag.AddError(openBracket.Location, lengthBetween, $"Type '{parent.TypeName}' is not readable by index");

                throw new RuntimeException();
            }

            var index = GetIndex(indexAccess.Index);

            try
            {
                return readable[index];
            }
            catch (IndexOutOfRangeException)
            {
                diagnosticBag.AddError(openBracket.Location, lengthBetween, "The index was outside the bounds of the list");

                throw new RuntimeException();
            }
        }

        public Obj Execute(BinaryExpression binary)
        {
            var opToken = binary.OperatorToken;
            
            var left = binary.Left.Accept(this);
            var right = binary.Right.Accept(this);
            var method = binaryOperations
                .Single(op => op.IsOperator(opToken.Type))
                .GetMethod(left, right);

            if (!method.IsUnknown) 
                return method.Invoke(left, right);
            
            diagnosticBag.AddError(opToken.Location, opToken.Length,
                $"The binary operator '{opToken.Text}' is not defined for '{left.TypeName}' and '{right.TypeName}' types");

            throw new RuntimeException();
        }

        public Obj Execute(UnaryExpression unary)
        {
            var opToken = unary.OperatorToken;
            var operand = unary.Operand.Accept(this);
            var method = unaryOperations
                .Single(op => op.IsOperator(opToken.Type))
                .GetMethod(operand);

            if (!method.IsUnknown) 
                return method.Invoke(operand);
            
            diagnosticBag.AddError(opToken.Location, opToken.Length,
                $"The unary operator '{opToken.Text}' is not defined for '{operand.TypeName}' type");
                
            throw new RuntimeException();
        }

        public Obj Execute(NumberExpression number) => new Number(number.Value);

        public Obj Execute(BooleanExpression boolean) => new Objects.Boolean(boolean.Value);

        public Obj Execute(StringExpression str) => new Objects.String(str.Value);
        
        public Obj Execute(ListExpression list)
        {
            var items = list.Items
                .Select(expression => expression.Accept(this))
                .ToImmutableArray();

            return new Objects.Array(items);
        }

        public Obj Execute(NullExpression expression) => new Null();

        public Obj Execute(VariableExpression variable)
        {
            if (scope.TryLookup(variable.Name.Text, out var value))
                return value;

            var nameToken = variable.Name;
            diagnosticBag.AddError(nameToken.Location, nameToken.Length,$"Variable '{nameToken.Text}' does not exist");

            throw new RuntimeException();
        }

        public Obj Execute(FunctionCallExpression expression)
        {
            var name = expression.Name;

            if (scope.TryLookup(name.Text, out var value) && value is Function function)
            {
                var arguments = EvaluateArguments(expression, function);
                
                return CallFunction(function, arguments);
            }
            
            diagnosticBag.AddError(name.Location, name.Length,$"Function '{name.Text}' does not exist");

            throw new RuntimeException();
        }

        private Obj CallFunction(Function function, ImmutableDictionary<string, Obj> arguments)
        {
            callStack.Push(function);

            var previousScope = scope;
            scope = new Scope(globalScope);
            
            foreach (var param in arguments)
                scope.Assign(param.Key, param.Value);

            Obj returnedValue = new Null();
            try
            {
                function.Call(new FunctionCallContext(function, scope));
            }
            catch (ReturnInterrupt interrupt)
            {
                returnedValue = interrupt.Value;
            }
            
            scope = previousScope;
            callStack.Pop();

            return returnedValue;
        }

        private ImmutableDictionary<string, Obj> EvaluateArguments(FunctionCallExpression expression, Function function)
        {
            var evaluatedArguments = ImmutableDictionary.CreateBuilder<string, Obj>();
            
            var arguments = expression.Arguments;
            var positions = function.PositionParameters;
            var defaults = function.DefaultParameters;

            var expectedLength = positions.Length + defaults.Length;
            if (!(arguments.Length >= positions.Length && arguments.Length <= expectedLength))
            {
                var location = expression.OpenParenthesis.Location;
                var lenght = expression.CloseParenthesis.Location.Position - location.Position + 1;
                var name = expression.Name.Text;
                
                diagnosticBag.AddError(location, lenght,
                    $"Function '{name}' requires {expectedLength} arguments but was given {arguments.Length}");

                throw new RuntimeException();
            }
            
            for (var i = 0; i < positions.Length; i++)
                evaluatedArguments[positions[i]] = arguments[i].Accept(this);

            for (var i = 0; i < arguments.Length - positions.Length; i++)
                evaluatedArguments[defaults[i].name] = arguments[i + positions.Length].Accept(this);

            for (var i = 0; i < positions.Length + defaults.Length - arguments.Length; i++)
            {
                var offset = i + arguments.Length - positions.Length;
                evaluatedArguments[defaults[offset].name] = defaults[offset].value;
            }

            return evaluatedArguments.ToImmutable();
        }

        private int GetIndex(SyntaxIndex syntaxIndex)
        {
            var openBracket = syntaxIndex.OpenBracket;
            var closeBracket = syntaxIndex.CloseBracket;
            var lengthBetween = closeBracket.Location.Position - openBracket.Location.Position + 1;
            
            var index = syntaxIndex.Index.Accept(this);
            
            if (index is not Number number)
            {
                diagnosticBag.AddError(openBracket.Location, lengthBetween, $"Expected number value but was '{index.TypeName}' type");

                throw new RuntimeException();
            }

            if (number.IsInteger) 
                return (int)number.Value;
            
            diagnosticBag.AddError(openBracket.Location, lengthBetween, "Expected integer value");
            throw new RuntimeException();
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
    }
}