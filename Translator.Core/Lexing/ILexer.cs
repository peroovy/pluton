using System.Collections.Immutable;
using Translator.Core.Text;

namespace Translator.Core.Lexing
{
    public interface ILexer
    {
        public ImmutableArray<SyntaxToken> Tokenize(ImmutableArray<Line> lines);
    }
}