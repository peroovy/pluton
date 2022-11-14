namespace Translator.Core.Lexing.TokenParsers.Single
{
    public class StarParser : SingleTerminalParser
    {
        public StarParser() : base(TokenTypes.Star, '*')
        {
        }
    }
}