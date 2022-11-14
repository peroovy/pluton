namespace Translator.Core.Lexing.TokenParsers.Single
{
    public class PlusParser : SingleTerminalParser
    {
        public PlusParser() : base(TokenTypes.Plus, '+')
        {
        }
    }
}