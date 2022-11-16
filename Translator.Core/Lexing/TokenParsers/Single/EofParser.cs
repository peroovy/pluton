namespace Translator.Core.Lexing.TokenParsers.Single
{
    public class EofParser : SingleTerminalParser
    {
        public EofParser() : base(TokenTypes.EOF, '\0')
        {
        }
    }
}