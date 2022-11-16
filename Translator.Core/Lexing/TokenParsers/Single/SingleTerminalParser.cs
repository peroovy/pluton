using Translator.Core.Logging;
using Translator.Core.Text;

namespace Translator.Core.Lexing.TokenParsers.Single
{
    public abstract class SingleTerminalParser : ITokenParser
    {
        private readonly TokenTypes type;
        private readonly char expectedValue;

        protected SingleTerminalParser(TokenTypes type, char expectedValue)
        {
            this.type = type;
            this.expectedValue = expectedValue;
        }

        public bool CanParseFrom(Line line, int position) => line.Value[position] == expectedValue;

        public SyntaxToken Parse(Line line, int position) =>
            new SyntaxToken(type, expectedValue.ToString(), new TextLocation(line, position));
    }
}