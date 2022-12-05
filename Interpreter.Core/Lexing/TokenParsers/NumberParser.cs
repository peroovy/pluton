using System.Text.RegularExpressions;
using Interpreter.Core.Text;

namespace Interpreter.Core.Lexing.TokenParsers
{
    public class NumberParser : ITokenParser
    {
        private readonly Regex regex = new(@"([0-9]*[.])?[0-9]+");

        public int Priority => 100;
        
        public SyntaxToken TryParse(Line line, int position)
        {
            var current = line.Value[position];
            var next = position + 1 < line.Length 
                ? line.Value[position + 1]
                : '\0';

            if (!(char.IsDigit(current) || current == '.' && char.IsDigit(next)))
                return null;
            
            var value = regex
                .Match(line.Value, position)
                .ToString();

            return new SyntaxToken(TokenTypes.Number, value, new TextLocation(line, position));
        }
    }
}