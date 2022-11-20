using Translator.Core.Lexing;

namespace Translator.Core.Execution.BinaryOperations
{
    public class SubtractionNumbersOperation : IBinaryOperation
    {
        public bool IsEvaluatedFor(Object left, TokenTypes operatorType, Object right)
        {
            return left.Type == typeof(double) && operatorType == TokenTypes.Minus && right.Type == typeof(double);
        }

        public Object Evaluate(Object left, Object right) => new Object((double)left.Value - (double)right.Value);
    }
}