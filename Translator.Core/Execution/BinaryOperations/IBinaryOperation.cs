using Translator.Core.Lexing;

namespace Translator.Core.Execution.BinaryOperations
{
    public interface IBinaryOperation
    {
        bool CanEvaluateForOperands(object left, TokenTypes operatorType, object right);
        
        object Evaluate(object left, object right);
    }
}