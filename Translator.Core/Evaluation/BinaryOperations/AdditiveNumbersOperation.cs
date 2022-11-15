using Translator.Core.Lexing;

namespace Translator.Core.Evaluation.BinaryOperations
{
    public class AdditiveNumbersOperation : IBinaryOperation
    {
        public bool CanEvaluateForOperands(object left, TokenTypes operatorType, object right)
        {
            return left is double && operatorType == TokenTypes.Plus && right is double;
        }

        public object Evaluate(object left, object right) => (double)left + (double)right;
    }
}