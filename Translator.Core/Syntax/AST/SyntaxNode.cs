using Translator.Core.Execution;
using Translator.Core.Execution.Objects;

namespace Translator.Core.Syntax.AST
{
    public abstract class SyntaxNode
    {
        public abstract Obj Accept(IExecutor executor);
    }
}