using Translator.Core.Lexing;

namespace Translator.Core.Execution.BinaryOperations
{
    public class DivisionNumbersOperation : IBinaryOperation
    {
        public bool IsEvaluatedFor(Object left, TokenTypes operatorType, Object right)
        {
            return left.Type == typeof(double) && operatorType == TokenTypes.Slash && right.Type == typeof(double);
        }

        public Object Evaluate(Object left, Object right)
        {
            var rightValue = (double)right.Value;

            return new Object(rightValue == 0 ? double.NaN : (double)left.Value / rightValue);
        }
    }
}