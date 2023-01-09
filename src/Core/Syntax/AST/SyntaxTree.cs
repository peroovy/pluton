using System.Collections.Immutable;

namespace Core.Syntax.AST
{
    public class SyntaxTree
    {
        public SyntaxTree(ImmutableArray<SyntaxNode> members)
        {
            Members = members;
        }
        
        public ImmutableArray<SyntaxNode> Members { get; }
    }
}