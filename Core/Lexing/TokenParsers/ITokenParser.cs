using Core.Text;

namespace Core.Lexing.TokenParsers
{
    public interface ITokenParser
    {
        public int Priority { get; }

        SyntaxToken TryParse(Line line, int position);
    }
}