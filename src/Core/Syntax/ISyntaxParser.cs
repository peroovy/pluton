using System.Collections.Generic;
using Core.Lexing;
using Core.Syntax.AST;
using Core.Utils.Text;

namespace Core.Syntax
{
    public interface ISyntaxParser
    {
        TranslationState<SyntaxTree> Parse(SourceText text, IEnumerable<SyntaxToken> tokens);
    }
}