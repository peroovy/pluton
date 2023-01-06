using System.Collections.Immutable;
using Core.Utils.Text;

namespace Core.Lexing
{
    public interface ILexer
    {
        TranslationState<ImmutableArray<SyntaxToken>> Tokenize(SourceText sourceText);
    }
}