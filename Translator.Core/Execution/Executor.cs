using System.Collections.Generic;
using System.Linq;
using Translator.Core.Execution.Objects;
using Translator.Core.Execution.Operation.Binary;
using Translator.Core.Logging;
using Translator.Core.Syntax.AST;
using Translator.Core.Syntax.AST.Expressions;

namespace Translator.Core.Execution
{
    public class Executor : IExecutor
    {
        private readonly BinaryOperation[] binaryOperations;
        private readonly ILogger logger;
        private readonly Scope scope = new(null);

        public Executor(BinaryOperation[] binaryOperations, ILogger logger)
        {
            this.binaryOperations = binaryOperations;
            this.logger = logger;
        }

        public Obj Execute(ExpressionStatement statement) => statement.Expression.Accept(this);

        public Obj Execute(AssignmentExpression assignment)
        {
            var name = assignment.Variable.Text;
            var value = assignment.Expression.Accept(this);

            if (value is Undefined)
                return value;

            var currentScope = scope;
            do
            {
                if (currentScope.Contains(name))
                    return currentScope.Assign(name, value);

                currentScope = currentScope.Parent;
            } 
            while (currentScope is not null);

            return scope.Assign(name, value);
        }

        public Obj Execute(ParenthesizedExpression expression) => expression.InnerExpression.Accept(this);

        public Obj Execute(BinaryExpression binary)
        {
            var opToken = binary.OperatorToken;
            
            var left = binary.Left.Accept(this);
            var right = binary.Right.Accept(this);
            var method = binaryOperations
                .First(op => op.IsOperator(opToken.Type))
                .GetMethod(left, right);

            if (method.IsUnknown)
            {
                logger.Error(opToken.Location, opToken.Length,
                    $"The binary operator '{opToken.Text}' is not defined for '{left.Type}' and '{right.Type}' types");

            }
            
            return method.Invoke(left, right);
        }

        public Obj Execute(NumberExpression number) => new Number(number.Value);

        public Obj Execute(BooleanExpression boolean) => new Boolean(boolean.Value);
        
        public Obj Execute(VariableExpression variable)
        {
            if (scope.TryLookup(variable.Name.Text, out var value))
                return value;

            var nameToken = variable.Name;
            logger.Error(nameToken.Location, nameToken.Length,$"Variable '{nameToken.Text}' does not exist");

            return new Undefined();
        }
    }
}