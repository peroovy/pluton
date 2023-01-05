using System;
using System.Text;
using System.Text.RegularExpressions;
using Core.Utils.Diagnostic;

namespace Core.Lexing.TokenParsers
{
    public class StringParser : ITokenParser
    {
        private const char Quote = '"';

        public Priority Priority => Priority.Low;
        
        public SyntaxToken TryParse(Line line, int position, DiagnosticBag diagnostic)
        {
            if (line[position] != Quote)
                return null;
            
            var str = line.Value;

            var stringValue = new StringBuilder();
            for (var i = position + 1; i < str.Length && str[i] != Quote; i++)
                stringValue.Append(str[i]);
            
            var endLimiterPosition = position + stringValue.Length + 1;
            if (endLimiterPosition < str.Length && str[endLimiterPosition] == Quote)
            {
                var lengthWithLimiters = stringValue.Length + 2;
                var tokenValue = ConvertEscapedCharacters(stringValue.ToString());
                var location = new Location(line, position, lengthWithLimiters);
                
                if (tokenValue is not null)
                    return new SyntaxToken(TokenType.String, tokenValue, location);
                
                diagnostic.AddError(location, "Unrecognized escape sequence");
                return null;
            }

            diagnostic.AddError(new Location(line, position, stringValue.Length + 1), "Unterminated string literal");
            return null;
        }

        private static string ConvertEscapedCharacters(string input)
        {
            try
            {
                return Regex.Unescape(input);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }
    }
}