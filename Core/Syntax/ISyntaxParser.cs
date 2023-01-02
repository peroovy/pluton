using System.Collections.Immutable;
using Core.Lexing;
using Core.Syntax.AST;

namespace Core.Syntax
{
    public interface ISyntaxParser
    {
        SyntaxTree Parse(ImmutableArray<SyntaxToken> tokens);
    }
}