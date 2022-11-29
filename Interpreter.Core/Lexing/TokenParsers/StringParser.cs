using System.Text;
using System.Text.RegularExpressions;
using Interpreter.Core.Logging;
using Interpreter.Core.Text;

namespace Interpreter.Core.Lexing.TokenParsers
{
    public class StringParser : ITokenParser
    {
        private const char Limiter = '"';
        private const char Escape = '\\';
        
        public int Priority => 0;

        public bool CanParseFrom(Line line, int position) => line.Value[position] == Limiter;

        public SyntaxToken Parse(Line line, int position, ILogger logger)
        {
            var builder = new StringBuilder();
            var lineValue = line.Value;
            var escapedCount = 0;
            var hasEndLimiter = false;

            for (var tail = position + 1; tail < lineValue.Length - 1; tail++)
            {
                var sym = lineValue[tail];
                
                if (sym == Escape && tail + 1 < lineValue.Length)
                {
                    var str = Regex.Unescape(string.Concat(Escape, lineValue[tail + 1]));
                    
                    builder.Append(str);
                    escapedCount++;
                    tail++;
                    continue;
                }
                
                if (sym == Limiter)
                {
                    hasEndLimiter = true;
                    break;
                }

                builder.Append(sym);
            }

            var location = new TextLocation(line, position);
            var length = builder.Length + 2 + escapedCount;

            if (hasEndLimiter) 
                return new SyntaxToken(TokenTypes.String, builder.ToString(), location, length);
            
            logger.Error(location, length - 1, "Unterminated string literal");
            return null;
        }
    }
}