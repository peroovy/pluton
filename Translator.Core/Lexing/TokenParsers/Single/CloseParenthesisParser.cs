namespace Translator.Core.Lexing.TokenParsers.Single
{
    public class CloseParenthesisParser : SingleTerminalParser
    {
        public CloseParenthesisParser() : base(TokenTypes.CloseParenthesis, ')')
        {
        }
    }
}