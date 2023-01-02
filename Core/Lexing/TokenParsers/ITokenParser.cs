using Core.Text;
using Core.Logging;

namespace Core.Lexing.TokenParsers
{
    public interface ITokenParser
    {
        public int Priority { get; }

        SyntaxToken TryParse(Line line, int position);
    }
}