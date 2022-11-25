using Interpreter.Core.Execution;
using Interpreter.Core.Execution.Objects;

namespace Interpreter.Core.Syntax.AST
{
    public abstract class SyntaxNode
    {
        public abstract Obj Accept(IExecutor executor);
    }
}