using System.Collections.Immutable;
using Core.Text;

namespace Core.Lexing
{
    public interface ILexer
    {
        ImmutableArray<SyntaxToken> Tokenize(ImmutableArray<Line> lines);
    }
}