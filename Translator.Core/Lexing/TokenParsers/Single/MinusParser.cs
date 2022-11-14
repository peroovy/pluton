namespace Translator.Core.Lexing.TokenParsers.Single
{
    public class MinusParser : SingleTerminalParser
    {
        public MinusParser() : base(TokenTypes.Minus, '-')
        {
        }
    }
}