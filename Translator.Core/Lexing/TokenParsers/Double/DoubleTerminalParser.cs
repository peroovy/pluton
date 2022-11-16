using Translator.Core.Logging;
using Translator.Core.Text;

namespace Translator.Core.Lexing.TokenParsers.Double
{
    public abstract class DoubleTerminalParser : ITokenParser
    {
        private readonly TokenTypes type;
        private readonly char first;
        private readonly char second;

        protected DoubleTerminalParser(TokenTypes type, char first, char second)
        {
            this.type = type;
            this.first = first;
            this.second = second;
        }

        public bool CanParseFrom(Line line, int position) => 
            string.Concat(line.Value.TakeFrom(position, 2)) == string.Concat(first, second);

        public SyntaxToken Parse(Line line, int position)
        {
            var lineValue = line.Value;

            return new SyntaxToken(type,
                string.Concat(lineValue[position], lineValue[position + 1]),
                new TextLocation(line, position)
            );
        }
    }
}