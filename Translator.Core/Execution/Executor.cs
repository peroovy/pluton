using System.Linq;
using Translator.Core.Execution.BinaryOperations;
using Translator.Core.Logging;
using Translator.Core.Syntax.AST;

namespace Translator.Core.Execution
{
    public class Executor : IExecutor
    {
        private readonly IBinaryOperation[] binaryOperations;
        private readonly ILogger logger;

        public Executor(IBinaryOperation[] binaryOperations, ILogger logger)
        {
            this.binaryOperations = binaryOperations;
            this.logger = logger;
        }

        public object Execute(ParenthesizedExpression expression) => expression.InnerExpression.Accept(this);

        public object Execute(BinaryExpression binary)
        {
            var op = binary.OperatorToken;
            
            var left = binary.Left.Accept(this);
            var right = binary.Right.Accept(this);
            var binaryOperation = binaryOperations.FirstOrDefault(operation =>
                operation.CanEvaluateForOperands(left, binary.OperatorToken.Type, right)
            );

            if (binaryOperation != null) 
                return binaryOperation.Evaluate(left, right);

            logger.Error(op.Location, op.Length,
                $"The binary operator '{op.Value}' is not defined for '{left.GetType().Name}' and '{right.GetType().Name}' types");

            return left;
        }

        public object Execute(NumberExpression number) => number.Value;

        public object Execute(BooleanExpression boolean) => boolean.Value;
    }
}