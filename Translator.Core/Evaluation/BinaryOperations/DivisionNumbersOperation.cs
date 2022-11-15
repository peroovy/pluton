using Translator.Core.Lexing;

namespace Translator.Core.Evaluation.BinaryOperations
{
    public class DivisionNumbersOperation : IBinaryOperation
    {
        public bool CanEvaluateForOperands(object left, TokenTypes operatorType, object right)
        {
            return left is double && operatorType == TokenTypes.Slash && right is double;
        }

        public object Evaluate(object left, object right)
        {
            var rightNumber = (double)right;

            if (rightNumber == 0)
                return null;

            return (double)left / rightNumber;
        }
    }
}