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
            var binaryOperation = binaryOperations.FirstOrDefault(operation =>
                operation.CanEvaluateForOperands(left, binary.OperatorToken.Type, right)
            );

            if (binaryOperation != null) 
                return binaryOperation.Evaluate(left, right);

            logger.Error(op.Location, op.Length,
                $"The binary operator '{op.Value}' is not defined for '{left.GetType().Name}' and '{right.GetType().Name}' types");

            return left;
        }

        public object Evaluate(NumberExpression number) => number.Value;

        public object Evaluate(BooleanExpression boolean) => boolean.Value;
    }
}