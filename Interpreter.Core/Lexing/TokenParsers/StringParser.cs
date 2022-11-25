using Translator.Core.Text;

namespace Translator.Core.Lexing.TokenParsers
{
    public class StringParser : ITokenParser
    {
        private const char Quote = '"';
        
        public bool CanParseFrom(Line line, int position) => line.Value[position] == Quote;

        public SyntaxToken Parse(Line line, int position)
        {
            var value = string.Concat(line.Value.TakeWhileFrom(sym => sym != Quote, position + 1));

            return new SyntaxToken(TokenTypes.String, value, new TextLocation(line, position));
        }
    }
}