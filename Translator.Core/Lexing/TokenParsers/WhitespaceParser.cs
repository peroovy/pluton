namespace Translator.Core.Lexing.TokenParsers
{
    public class WhitespaceParser : ITokenParser
    {
        public bool IsStartingFrom(string code, int position) => char.IsWhiteSpace(code[position]);

        public SyntaxToken Parse(string code, int position)
        {
            if (code[position] == '\n')
                return new SyntaxToken(TokenTypes.LineSeparator, "\n");

            var value = string.Concat(
                code.TakeWhileFrom(sym => char.IsWhiteSpace(sym) && sym != '\n', position)
            );

            return new SyntaxToken(TokenTypes.Space, value);
        }
    }
}