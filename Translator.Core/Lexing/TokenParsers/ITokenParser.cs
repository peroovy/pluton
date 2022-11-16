using Translator.Core.Text;

namespace Translator.Core.Lexing.TokenParsers
{
    public interface ITokenParser
    {
        bool CanParseFrom(Line line, int position);

        SyntaxToken Parse(Line line, int position);
    }
}