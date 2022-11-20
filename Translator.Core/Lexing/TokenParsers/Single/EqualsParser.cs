namespace Translator.Core.Lexing.TokenParsers.Single
{
    public class EqualsParser : SingleTerminalParser
    {
        public EqualsParser() : base(TokenTypes.Equals, '=')
        {
        }
    }
}