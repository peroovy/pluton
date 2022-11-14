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

        public bool IsStartingFrom(string code, int position) => code[position] == expectedValue;

        public SyntaxToken Parse(string code, int position) => new SyntaxToken(type, expectedValue.ToString());
    }
}