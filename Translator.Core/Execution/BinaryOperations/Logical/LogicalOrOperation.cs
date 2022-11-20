using Translator.Core.Lexing;

namespace Translator.Core.Execution.BinaryOperations.Logical
{
    public class LogicalOrOperation : IBinaryOperation
    {
        public bool CanEvaluateForOperands(object left, TokenTypes operatorType, object right)
        {
            return left is bool && operatorType == TokenTypes.DoublePipe && right is bool;
        }

        public object Evaluate(object left, object right) => (bool)left || (bool)right;
    }
}