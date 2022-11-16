namespace Translator.Core.Lexing.TokenParsers.Double
{
    public class LogicalOrParser : DoubleTerminalParser
    {
        public LogicalOrParser() : base(TokenTypes.DoubleAmpersand, '|', '|')
        {
        }
    }
}