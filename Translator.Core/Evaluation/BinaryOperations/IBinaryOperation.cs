using System;
using Translator.Core.Lexing;

namespace Translator.Core.Evaluation.BinaryOperations
{
    public interface IBinaryOperation
    {
        bool CanEvaluateForOperands(object left, TokenTypes operatorType, object right);
        
        object Evaluate(object left, object right);
    }
}