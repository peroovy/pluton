using Translator.Core.Lexing;

namespace Translator.Core.Execution.BinaryOperations
{
    public interface IBinaryOperation
    {
        bool IsEvaluatedFor(Object left, TokenTypes operatorType, Object right);
        
        Object Evaluate(Object left, Object right);
    }
}