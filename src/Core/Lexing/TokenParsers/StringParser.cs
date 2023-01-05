using System;
using System.Text;
using System.Text.RegularExpressions;
using Core.Utils.Diagnostic;
using Core.Utils.Text;

namespace Core.Lexing.TokenParsers
{
    public class StringParser : ITokenParser
    {
        private readonly IDiagnosticBag diagnosticBag;
        
        private const char Limiter = '"';

        public StringParser(IDiagnosticBag diagnosticBag)
        {
            this.diagnosticBag = diagnosticBag;
        }
        
        public int Priority => 0;
        
        public SyntaxToken TryParse(Line line, int position)
        {
            if (line[position] != Limiter)
                return null;
            
            var str = line.Value;

            var stringValue = new StringBuilder();
            for (var i = position + 1; i < str.Length && str[i] != Limiter; i++)
                stringValue.Append(str[i]);
            
            var endLimiterPosition = position + stringValue.Length + 1;
            if (endLimiterPosition < str.Length && str[endLimiterPosition] == Limiter)
            {
                var lengthWithLimiters = stringValue.Length + 2;
                var tokenValue = ConvertEscapedCharacters(stringValue.ToString());
                var location = new Location(line, position, lengthWithLimiters);
                
                if (tokenValue is not null)
                    return new SyntaxToken(TokenTypes.String, tokenValue, location);
                
                diagnosticBag.AddError(location, "Unrecognized escape sequence");
                return null;
            }
            
            diagnosticBag.AddError(new Location(line, position, stringValue.Length + 1), "Unterminated string literal");
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