using System;
using System.Text;
using System.Text.RegularExpressions;
using Core.Diagnostic;
using Core.Text;

namespace Core.Lexing.TokenParsers
{
    public class StringParser : ITokenParser
    {
        private readonly IDiagnosticBag diagnosticBag;
        
        private const char Limiter = '"';
        private const char EscapeCharacter = '\\';

        public StringParser(IDiagnosticBag diagnosticBag)
        {
            this.diagnosticBag = diagnosticBag;
        }
        
        public int Priority => 0;
        
        public SyntaxToken TryParse(Line line, int position)
        {
            if (line.Value[position] != Limiter)
                return null;
            
            var str = line.Value;

            var value = new StringBuilder();
            for (var i = position + 1; i < str.Length && str[i] != Limiter; i++)
                value.Append(str[i]);
            
            var location = new TextLocation(line, position);
            var endLimiterPosition = position + value.Length + 1;

            if (endLimiterPosition < str.Length && str[endLimiterPosition] == Limiter)
            {
                var lengthWithLimiters = value.Length + 2;
                var tokenValue = ConvertEscapedCharacters(value.ToString());
                
                if (tokenValue is not null)
                    return new SyntaxToken(TokenTypes.String, tokenValue, location, lengthWithLimiters);
                
                diagnosticBag.AddError(location, lengthWithLimiters, "Unrecognized escape sequence");
                return null;
            }
            
            diagnosticBag.AddError(location, value.Length + 1, "Unterminated string literal");
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