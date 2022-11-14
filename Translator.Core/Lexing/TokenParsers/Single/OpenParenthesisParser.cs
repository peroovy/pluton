namespace Translator.Core.Lexing.TokenParsers.Single
{
    public class OpenParenthesisParser : SingleTerminalParser
    {
        public OpenParenthesisParser() : base(TokenTypes.OpenParenthesis, '(')
        {
        }
    }
}