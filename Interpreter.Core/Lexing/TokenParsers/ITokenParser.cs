using Interpreter.Core.Logging;
using Interpreter.Core.Text;

namespace Interpreter.Core.Lexing.TokenParsers
{
    public interface ITokenParser
    {
        public int Priority { get; }

        SyntaxToken TryParse(Line line, int position);
    }
}