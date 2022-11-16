namespace Translator.Core.Lexing.TokenParsers.Double
{
    public class LogicalAndParser : DoubleTerminalParser
    {
        public LogicalAndParser() : base(TokenTypes.DoubleAmpersand, '&', '&')
        {
        }
    }
}