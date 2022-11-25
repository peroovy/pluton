﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Translator.Core.Execution.Objects;
using Translator.Core.Execution.Objects.BuiltinFunctions;
using Translator.Core.Execution.Operations.Binary;
using Translator.Core.Execution.Operations.Unary;
using Translator.Core.Logging;
using Translator.Core.Syntax.AST;
using Translator.Core.Syntax.AST.Expressions;
using Boolean = Translator.Core.Execution.Objects.Boolean;

namespace Translator.Core.Execution
{
    public class Executor : IExecutor
    {
        private readonly BinaryOperation[] binaryOperations;
        private readonly UnaryOperation[] unaryOperations;
        private readonly ILogger logger;

        private readonly Stack<Obj> stack = new();
        private Scope currentScope = new(null);

        public Executor(BinaryOperation[] binaryOperations, UnaryOperation[] unaryOperations, ILogger logger)
        {
            this.binaryOperations = binaryOperations;
            this.unaryOperations = unaryOperations;
            this.logger = logger;

            foreach (var function in BuiltinFunctions.GetAll())
                currentScope.Assign(function.Name, function);
        }

        public Obj Execute(FunctionDeclarationStatement statement)
        {
            var function = new Function(
                statement.Name.Text,
                statement.PositionParameters.Select(p => p.Text).ToImmutableArray(),
                (func, _) =>
                {
                    var enumerator = statement.Body.Statements.GetEnumerator();
                    while (stack.Peek() == func && enumerator.MoveNext())
                        enumerator.Current.Accept(this);
                },
                isBuiltin: false
            );

            currentScope.Assign(function.Name, function);

            return null;
        }

        public Obj Execute(ReturnStatement statement)
        {
            if (stack.Count > 0 && stack.Peek() is Function)
            {
                var expression = statement.Expression?.Accept(this) ?? new Undefined();
                stack.Pop();
                stack.Push(expression);

                return null;
            }

            var keyword = statement.Keyword;
            logger.Error(keyword.Location, keyword.Length, $"The 'return' statement can be only into function block");
            
            return null;
        }

        public Obj Execute(ForStatement statement)
        {
            var previousScope = currentScope;
            currentScope = new Scope(previousScope);
            
            foreach (var initializer in statement.Initializers)
                initializer.Accept(this);

            while (statement.Condition.Accept(this).ToBoolean().IsTrue)
            {
                statement.Body.Accept(this);

                foreach (var iterator in statement.Iterators)
                    iterator.Accept(this);
            }

            currentScope = previousScope;

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
            var parentScope = currentScope;
            currentScope = new Scope(parentScope);
            
            foreach (var statement in block.Statements)
                statement.Accept(this);

            currentScope = parentScope;

            return null;
        }

        public Obj Execute(ExpressionStatement statement)
        {
            statement.Expression.Accept(this);

            return null;
        }

        public Obj Execute(AssignmentExpression assignment)
        {
            var name = assignment.Variable.Text;
            var value = assignment.Expression.Accept(this);

            if (value is Undefined)
            {
                var op = assignment.OperatorToken;
                logger.Error(op.Location, op.Length, "Expression must have a value");

                return null;
            }

            if (!TryAssignUp(name, value))
                currentScope.Assign(name, value);

            return value;
        }

        public Obj Execute(ParenthesizedExpression expression) => expression.InnerExpression.Accept(this);

        public Obj Execute(BinaryExpression binary)
        {
            var opToken = binary.OperatorToken;
            
            var left = binary.Left.Accept(this);
            var right = binary.Right.Accept(this);
            var method = binaryOperations
                .Single(op => op.IsOperator(opToken.Type))
                .GetMethod(left, right);

            if (method.IsUnknown)
            {
                logger.Error(opToken.Location, opToken.Length,
                    $"The binary operator '{opToken.Text}' is not defined for '{left.Type}' and '{right.Type}' types");
            }
            
            return method.Invoke(left, right);
        }

        public Obj Execute(UnaryExpression unary)
        {
            var opToken = unary.OperatorToken;
            var operand = unary.Operand.Accept(this);
            var method = unaryOperations
                .Single(op => op.IsOperator(opToken.Type))
                .GetMethod(operand);
            
            if (method.IsUnknown)
            {
                logger.Error(opToken.Location, opToken.Length,
                    $"The unary operator '{opToken.Text}' is not defined for '{operand.Type}' type");
            }

            return method.Invoke(operand);
        }

        public Obj Execute(NumberExpression number) => new Number(number.Value);

        public Obj Execute(BooleanExpression boolean) => new Boolean(boolean.Value);
        
        public Obj Execute(VariableExpression variable)
        {
            if (currentScope.TryLookup(variable.Name.Text, out var value))
                return value;

            var nameToken = variable.Name;
            logger.Error(nameToken.Location, nameToken.Length,$"Variable '{nameToken.Text}' does not exist");

            return new Undefined();
        }

        public Obj Execute(FunctionCallExpression expression)
        {
            var name = expression.Name;

            if (currentScope.TryLookup(name.Text, out var value) && value is Function function)
            {
                var actualCount = expression.PositionArguments.Length;
                var expectedCount = function.PositionArguments.Length;
                if (actualCount == expectedCount) 
                    return CallFunction(function, expression.PositionArguments);
                
                var location = expression.OpenParenthesis.Location;
                var lenght = expression.CloseParenthesis.Location.Position - location.Position + 1;
                logger.Error(location, lenght,
                    $"Function '{name.Text}' requires {expectedCount} arguments but was given {actualCount}");

                return new Undefined();
            }
            
            logger.Error(name.Location, name.Length,$"Function '{name.Text}' does not exist");

            return new Undefined();
        }

        private Obj CallFunction(Function function, ImmutableArray<Expression> arguments)
        {
            stack.Push(function);
            var previousScope = currentScope;
            currentScope = new Scope(previousScope);
            for (var i = 0; i < function.PositionArguments.Length; i++)
            {
                var parameter = function.PositionArguments[i];
                var parameterValue = arguments[i].Accept(this);
                currentScope.Assign(parameter, parameterValue);
            }

            function.Call(function, currentScope);
            currentScope = previousScope;

            var obj = stack.Pop();
            return obj == function ? new Undefined() : obj;
        }

        private bool TryAssignUp(string name, Obj value)
        {
            var current = currentScope;
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