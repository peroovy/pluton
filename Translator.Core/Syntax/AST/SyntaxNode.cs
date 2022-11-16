using Translator.Core.Evaluation;

namespace Translator.Core.Syntax.AST
{
    public abstract class SyntaxNode
    {
        public abstract object Accept(IEvaluator evaluator);
    }
}