using System.Collections.Generic;
using System.Linq;
using Translator.Core.Execution.BinaryOperations;
using Translator.Core.Execution.Objects;
using Translator.Core.Logging;
using Translator.Core.Syntax.AST;
using Translator.Core.Syntax.AST.Expressions;

namespace Translator.Core.Execution
{
    public class Executor : IExecutor
    {
        private readonly BinaryOperation[] binaryOperations;
        private readonly ILogger logger;
        private readonly Dictionary<string, Obj> variables = new();

        public Executor(BinaryOperation[] binaryOperations, ILogger logger)
        {
            this.binaryOperations = binaryOperations;
            this.logger = logger;
        }

        public Obj Execute(ExpressionStatement statement) => statement.Expression.Accept(this);

        public Obj Execute(AssignmentExpression assignment)
        {
            return variables[assignment.Variable.Value] = assignment.Expression.Accept(this);
        }

        public Obj Execute(ParenthesizedExpression expression) => expression.InnerExpression.Accept(this);

        public Obj Execute(BinaryExpression binary)
        {
            var opToken = binary.OperatorToken;
            
            var left = binary.Left.Accept(this);
            var right = binary.Right.Accept(this);
            var operation = binaryOperations
                .First(op => op.IsOperator(opToken.Type))
                .GetMethod(left, right);

            if (operation is null)
            {
                logger.Error(opToken.Location, opToken.Length,
                    $"The binary operator '{opToken.Value}' is not defined for '{left.Type}' and '{right.Type}' types");

            }
            
            return BinaryOperation.Evaluate(operation, left, right);
        }

        public Obj Execute(NumberExpression number) => new Number(number.Value);

        public Obj Execute(BooleanExpression boolean) => new Boolean(boolean.Value);
        
        public Obj Execute(VariableExpression variable)
        {
            if (variables.TryGetValue(variable.Name.Value, out var value))
                return value;

            var nameToken = variable.Name;
            logger.Error(nameToken.Location, nameToken.Length,$"Undeclared variable '{nameToken.Value}'");

            return new Undefined();
        }
    }
}