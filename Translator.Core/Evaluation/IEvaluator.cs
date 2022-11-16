using Translator.Core.Syntax.AST;

namespace Translator.Core.Evaluation
{
    public interface IEvaluator
    {
        object Evaluate(ParenthesizedExpression expression);
        
        object Evaluate(BinaryExpression binary);

        object Evaluate(NumberExpression number);
    }
}