using System.Collections.Immutable;
using Translator.Core.Lexing;
using Translator.Core.Syntax.AST;

namespace Translator.Core.Syntax
{
    public interface ISyntaxParser
    {
        SyntaxNode Parse(ImmutableArray<SyntaxToken> tokens);
    }
}