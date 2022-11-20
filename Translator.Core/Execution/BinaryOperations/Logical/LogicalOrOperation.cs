using Translator.Core.Lexing;

namespace Translator.Core.Execution.BinaryOperations.Logical
{
    public class LogicalOrOperation : IBinaryOperation
    {
        public bool IsEvaluatedFor(Object left, TokenTypes operatorType, Object right)
        {
            return left.Type == typeof(bool) && operatorType == TokenTypes.DoublePipe && right.Type == typeof(bool);
        }

        public Object Evaluate(Object left, Object right) => new Object((bool)left.Value && (bool)right.Value);
    }
}