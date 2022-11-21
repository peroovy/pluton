namespace Translator.Core.Lexing.TokenParsers.Single
{
    public class SemicolonParser : SingleTerminalParser
    {
        public SemicolonParser() : base(TokenTypes.Semicolon, ';')
        {
        }
    }
}