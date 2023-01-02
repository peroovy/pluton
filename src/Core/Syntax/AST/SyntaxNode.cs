using Core.Execution;
using Core.Execution.Objects;

namespace Core.Syntax.AST
{
    public abstract class SyntaxNode
    {
        public abstract Obj Accept(IExecutor executor);
    }
}