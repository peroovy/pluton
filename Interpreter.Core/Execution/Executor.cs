using System;
using System.Collections.Immutable;
using System.Linq;
using Interpreter.Core.Execution.Objects;
using Interpreter.Core.Execution.Objects.BuiltinFunctions;
using Interpreter.Core.Execution.Objects.MagicMethods;
using Interpreter.Core.Execution.Operations.Binary;
using Interpreter.Core.Execution.Operations.Unary;
using Interpreter.Core.Logging;
using Interpreter.Core.Syntax.AST;
using Interpreter.Core.Syntax.AST.Expressions;
using Array = Interpreter.Core.Execution.Objects.Array;
using Boolean = Interpreter.Core.Execution.Objects.Boolean;
using String = Interpreter.Core.Execution.Objects.String;

namespace Interpreter.Core.Execution
{
    public class Executor : IExecutor
    {
        private readonly BinaryOperation[] binaryOperations;
        private readonly UnaryOperation[] unaryOperations;
        private readonly ILogger logger;

        private readonly Stack stack = new();
        private Scope scope = new(null);

        public Executor(
            BinaryOperation[] binaryOperations, 
            UnaryOperation[] unaryOperations, 
            BuiltinFunction[] builtinFunctions, 
            ILogger logger)
        {
            this.binaryOperations = binaryOperations;
            this.unaryOperations = unaryOperations;
            this.logger = logger;

            foreach (var function in builtinFunctions)
                scope.Assign(function.Name, function);
        }

        public void Execute(SyntaxTree tree)
        {
            try
            {
                foreach (var member in tree.Members)
                    member.Accept(this);
            }
            catch (RuntimeException)
            {
            }
        }

        public Obj Execute(FunctionDeclarationStatement statement)
        {
            var function = new Function(
                statement.Name.Text,
                statement.PositionParameters.Select(p => p.Text).ToImmutableArray(),
                (func, _, _) =>
                {
                    var enumerator = statement.Body.Statements.GetEnumerator();
                    while (stack.Peek() == func && enumerator.MoveNext())
                        enumerator.Current.Accept(this);
                },
                isBuiltin: false
            );

            scope.Assign(function.Name, function);

            return null;
        }

        public Obj Execute(ReturnStatement statement)
        {
            if (stack.Count > 0 && stack.Peek() is Function)
            {
                var expression = statement.Expression?.Accept(this) ?? new Null();
                stack.PushFunctionResult(expression);

                return null;
            }

            var keyword = statement.Keyword;
            logger.Error(keyword.Location, keyword.Length, "The return statement can be only into function block");
            
            throw new RuntimeException(keyword.Location);
        }

        public Obj Execute(ForStatement statement)
        {
            var previousScope = scope;
            scope = new Scope(previousScope);
            
            foreach (var initializer in statement.Initializers)
                initializer.Accept(this);

            while (statement.Condition.Accept(this).ToBoolean().IsTrue)
            {
                statement.Body.Accept(this);

                foreach (var iterator in statement.Iterators)
                    iterator.Accept(this);
            }

            scope = previousScope;

            return null;
        }

        public Obj Execute(WhileStatement statement)
        {
            while (statement.Condition.Accept(this).ToBoolean().IsTrue)
                statement.Body.Accept(this);

            return null;
        }

        public Obj Execute(IfStatement statement)
        {
            if (statement.Condition.Accept(this).ToBoolean().IsTrue)
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
            statement.Expression.Accept(this);

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
                logger.Error(openBracket.Location, length, $"Type '{obj.Type}' is not settable by index");

                throw new RuntimeException(openBracket.Location);
            }

            var index = GetIndex(assignment.Index);
            var value = assignment.Value.Accept(this);

            try
            {
                return settable[index] = value;
            }
            catch (IndexOutOfRangeException)
            {
                logger.Error(openBracket.Location, length, "The index was outside the bounds of the list");

                throw new RuntimeException(openBracket.Location);
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
                logger.Error(openBracket.Location, lengthBetween, $"Type '{parent.Type}' is not readable by index");

                throw new RuntimeException(openBracket.Location);
            }

            var index = GetIndex(indexAccess.Index);

            try
            {
                return readable[index];
            }
            catch (IndexOutOfRangeException)
            {
                logger.Error(openBracket.Location, lengthBetween, "The index was outside the bounds of the list");

                throw new RuntimeException(openBracket.Location);
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
            
            logger.Error(opToken.Location, opToken.Length,
                $"The binary operator '{opToken.Text}' is not defined for '{left.Type}' and '{right.Type}' types");

            throw new RuntimeException(opToken.Location);
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
            
            logger.Error(opToken.Location, opToken.Length,
                $"The unary operator '{opToken.Text}' is not defined for '{operand.Type}' type");
                
            throw new RuntimeException(opToken.Location);
        }

        public Obj Execute(NumberExpression number) => new Number(number.Value);

        public Obj Execute(BooleanExpression boolean) => new Boolean(boolean.Value);

        public Obj Execute(StringExpression str) => new String(str.Value);
        
        public Obj Execute(ListExpression list)
        {
            var items = list.Items
                .Select(expression => expression.Accept(this))
                .ToImmutableArray();

            return new Array(items);
        }

        public Obj Execute(NullExpression expression) => new Null();

        public Obj Execute(VariableExpression variable)
        {
            if (scope.TryLookup(variable.Name.Text, out var value))
                return value;

            var nameToken = variable.Name;
            logger.Error(nameToken.Location, nameToken.Length,$"Variable '{nameToken.Text}' does not exist");

            throw new RuntimeException(nameToken.Location);
        }

        public Obj Execute(FunctionCallExpression expression)
        {
            var name = expression.Name;

            if (scope.TryLookup(name.Text, out var value) && value is Function function)
            {
                var actualCount = expression.PositionArguments.Length;
                var expectedCount = function.PositionParameters.Length;
                if (actualCount == expectedCount) 
                    return CallFunction(function, expression.PositionArguments);
                
                var location = expression.OpenParenthesis.Location;
                var lenght = expression.CloseParenthesis.Location.Position - location.Position + 1;
                logger.Error(location, lenght,
                    $"Function '{name.Text}' requires {expectedCount} arguments but was given {actualCount}");

                throw new RuntimeException(location);
            }
            
            logger.Error(name.Location, name.Length,$"Function '{name.Text}' does not exist");

            throw new RuntimeException(name.Location);
        }

        private Obj CallFunction(Function function, ImmutableArray<Expression> arguments)
        {
            stack.PushFunction(function);
            var evaluatedParams = function.PositionParameters
                .Zip(arguments, (name, expression) => (nameParam: name, argument: expression.Accept(this)))
                .ToImmutableArray();
            
            var previousScope = scope;
            scope = new Scope(previousScope);
            
            foreach (var param in evaluatedParams)
                scope.Assign(param.nameParam, param.argument);
            
            function.Call(function, scope, stack);
            scope = previousScope;

            var obj = stack.Pop();
            return obj == function ? new Null() : obj;
        }
        
        private int GetIndex(SyntaxIndex syntaxIndex)
        {
            var openBracket = syntaxIndex.OpenBracket;
            var closeBracket = syntaxIndex.CloseBracket;
            var lengthBetween = closeBracket.Location.Position - openBracket.Location.Position + 1;
            
            var index = syntaxIndex.Index.Accept(this);
            
            if (index is not Number number)
            {
                logger.Error(openBracket.Location, lengthBetween, $"Expected number value but was '{index.Type}' type");

                throw new RuntimeException(openBracket.Location);
            }

            if (number.IsInteger) 
                return (int)number.ToDouble();
            
            logger.Error(openBracket.Location, lengthBetween, "Expected integer value");
            throw new RuntimeException(openBracket.Location);
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