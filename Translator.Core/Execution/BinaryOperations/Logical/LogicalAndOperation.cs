using Translator.Core.Lexing;

namespace Translator.Core.Execution.BinaryOperations.Logical
{
    public class LogicalAndOperation : IBinaryOperation
    {
        public bool IsEvaluatedFor(Object left, TokenTypes operatorType, Object right)
        {
            return left.Type == typeof(bool) && operatorType == TokenTypes.DoubleAmpersand &&
                   right.Type == typeof(bool);
        }

        public Object Evaluate(Object left, Object right) => new Object((bool)left.Value && (bool)right.Value);
    }
}