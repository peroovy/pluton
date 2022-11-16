using Translator.Core.Lexing;

namespace Translator.Core.Evaluation.BinaryOperations.Logical
{
    public class LogicalAndOperation : IBinaryOperation
    {
        public bool CanEvaluateForOperands(object left, TokenTypes operatorType, object right)
        {
            return left is bool && operatorType == TokenTypes.DoubleAmpersand && right is bool;
        }

        public object Evaluate(object left, object right) => (bool)left && (bool)right;
    }
}