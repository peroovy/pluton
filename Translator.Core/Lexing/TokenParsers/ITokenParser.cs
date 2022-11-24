using Translator.Core.Text;

namespace Translator.Core.Lexing.TokenParsers
{
    public interface ITokenParser
    {
        public int Priority { get; }
        
        bool CanParseFrom(Line line, int position);

        SyntaxToken Parse(Line line, int position);
    }
}