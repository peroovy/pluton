using System.Collections.Immutable;
using Core.Execution;
using Core.Execution.Objects;

namespace Core.Syntax.AST
{
    public class SyntaxTree
    {
        public SyntaxTree(ImmutableArray<SyntaxNode> members)
        {
            Members = members;
        }
        
        public ImmutableArray<SyntaxNode> Members { get; }

        public Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}