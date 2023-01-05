using System.Collections.Generic;
using Core.Lexing;
using Core.Syntax.AST;

namespace Core.Syntax
{
    public interface ISyntaxParser
    {
        TranslationState<SyntaxTree> Parse(IEnumerable<SyntaxToken> tokens);
    }
}