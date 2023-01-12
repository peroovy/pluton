using System;
using System.Text;
using System.Text.RegularExpressions;
using Core.Utils.Diagnostic;
using Core.Utils.Text;

namespace Core.Lexing.TokenParsers
{
    public class StringParser : ITokenParser
    {
        private const char QuotationMark = '"';
        private const char Slash = '\\';

        public Priority Priority => Priority.Low;
        
        public SyntaxToken TryParse(SourceText text, int position, DiagnosticBag diagnostic)
        {
            if (text[position] != QuotationMark)
                return null;
            
            var stringValue = new StringBuilder();
            for (var i = position + 1; i < text.Length && (text[i] != QuotationMark || text[i - 1] == Slash); i++)
                stringValue.Append(text[i]);
            
            var endLimiterPosition = position + stringValue.Length + 1;
            if (endLimiterPosition < text.Length && text[endLimiterPosition] == QuotationMark)
            {
                var lengthWithLimiters = stringValue.Length + 2;
                var location = new Location(text, position, lengthWithLimiters);
                
                if (TryConvertToUnescaped(stringValue.ToString(), out var tokenValue))
                    return new SyntaxToken(TokenType.String, tokenValue, location);
                
                diagnostic.AddError(location, "Unrecognized escape sequence");
                return null;
            }

            diagnostic.AddError(new Location(text, position, stringValue.Length + 1), "Unterminated string literal");
            
            return null;
        }

        private static bool TryConvertToUnescaped(string input, out string result)
        {
            try
            {
                result = Regex.Unescape(input);
                return true;
            }
            catch (ArgumentException)
            {
                result = null;
                return false;
            }
        }
    }
}