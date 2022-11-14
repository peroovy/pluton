namespace Translator.Core.Lexing.TokenParsers.Single
{
    public class SlashParser : SingleTerminalParser
    {
        public SlashParser() : base(TokenTypes.Slash, '/')
        {
        }
    }
}