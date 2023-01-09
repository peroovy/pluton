using System;
using System.Text;
using System.Text.RegularExpressions;
using Core.Utils.Diagnostic;
using Core.Utils.Text;

namespace Core.Lexing.TokenParsers
{
    public class StringParser : ITokenParser
    {
        private const char Quote = '"';

        public Priority Priority => Priority.Low;
        
        public SyntaxToken TryParse(SourceText text, int position, DiagnosticBag diagnostic)
        {
            if (text[position] != Quote)
                return null;
            
            var stringValue = new StringBuilder();
            for (var i = position + 1; i < text.Length && text[i] != Quote; i++)
                stringValue.Append(text[i]);
            
            var endLimiterPosition = position + stringValue.Length + 1;
            if (endLimiterPosition < text.Length && text[endLimiterPosition] == Quote)
            {
                var lengthWithLimiters = stringValue.Length + 2;
                var tokenValue = ConvertEscapedCharacters(stringValue.ToString());
                var location = new Location(text, position, lengthWithLimiters);
                
                if (tokenValue is not null)
                    return new SyntaxToken(TokenType.String, tokenValue, location);
                
                diagnostic.AddError(location, "Unrecognized escape sequence");
                return null;
            }

            diagnostic.AddError(new Location(text, position, stringValue.Length + 1), "Unterminated string literal");
            
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