using System.Collections.Immutable;
using Core.Execution;

namespace Core.Syntax.AST
{
    public class SyntaxTree
    {
        public SyntaxTree(ImmutableArray<SyntaxNode> members)
        {
            Members = members;
        }
        
        public ImmutableArray<SyntaxNode> Members { get; }

        public void Accept(IExecutor executor) => executor.Execute(this);
    }
}