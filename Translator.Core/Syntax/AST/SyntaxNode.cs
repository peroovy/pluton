using Translator.Core.Execution;

namespace Translator.Core.Syntax.AST
{
    public abstract class SyntaxNode
    {
        public abstract Object Accept(IExecutor executor);
    }
}