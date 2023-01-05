using System.Text.RegularExpressions;
using Core.Utils.Diagnostic;

namespace Core.Lexing.TokenParsers
{
    public class NumberParser : ITokenParser
    {
        private readonly Regex regex = new(@"([0-9]*[.])?[0-9]+");

        public int Priority => 100;
        
        public SyntaxToken TryParse(Line line, int position, DiagnosticBag diagnostic)
        {
            var current = line[position];
            var next = position + 1 < line.Length ? line[position + 1] : '\0';

            if (!(char.IsDigit(current) || current == '.' && char.IsDigit(next)))
                return null;
            
            var value = regex
                .Match(line.Value, position)
                .ToString();

            var location = new Location(line, position, value.Length);
            return new SyntaxToken(TokenType.Number, value, location);
        }
    }
}