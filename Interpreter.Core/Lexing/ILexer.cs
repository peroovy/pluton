using System.Collections.Immutable;
using Interpreter.Core.Text;

namespace Interpreter.Core.Lexing
{
    public interface ILexer
    {
        public ImmutableArray<SyntaxToken> Tokenize(ImmutableArray<Line> lines);
    }
}