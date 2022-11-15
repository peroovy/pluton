using Translator.Core.Lexing;

namespace Translator.Core.Evaluation.BinaryOperations
{
    public class SubtractionNumbersOperation : IBinaryOperation
    {
        public bool CanEvaluateForOperands(object left, TokenTypes operatorType, object right)
        {
            return left is double && operatorType == TokenTypes.Minus && right is double;
        }

        public object Evaluate(object left, object right) => (double)left - (double)right;
    }
}