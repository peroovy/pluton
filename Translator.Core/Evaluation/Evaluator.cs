using System.Linq;
using Translator.Core.Evaluation.BinaryOperations;
using Translator.Core.Logging;
using Translator.Core.Syntax.AST;

namespace Translator.Core.Evaluation
{
    public class Evaluator : IEvaluator
    {
        private readonly IBinaryOperation[] binaryOperations;
        private readonly ILogger logger;

        public Evaluator(IBinaryOperation[] binaryOperations, ILogger logger)
        {
            this.binaryOperations = binaryOperations;
            this.logger = logger;
        }

        public object Evaluate(ParenthesizedExpression expression) => expression.InnerExpression.Accept(this);

        public object Evaluate(BinaryExpression binary)
        {
            var op = binary.OperatorToken;
            
            var left = binary.Left.Accept(this);
            var right = binary.Right.Accept(this);
            var operation = binaryOperations.FirstOrDefault(binaryOperation =>
                binaryOperation.CanEvaluateForOperands(left, binary.OperatorToken.Type, right)
            );

            if (operation is null)
            {
                logger.Error(op.Location, op.Length,
                    $"The binary operation '{op.Value} is not defined for types '{binary.Left.GetType()}' and '{binary.Right.GetType()}'");

                return right;
            }

            return operation.Evaluate(left, right);
        }

        public object Evaluate(NumberExpression number) => number.Value;
    }
}