using System.Collections.Immutable;

namespace Core.Lexing
{
    public interface ILexer
    {
        TranslationState<ImmutableArray<SyntaxToken>> Tokenize(string text);
    }
}