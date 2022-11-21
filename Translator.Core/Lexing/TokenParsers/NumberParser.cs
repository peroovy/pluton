using System.Text.RegularExpressions;
using Translator.Core.Text;

namespace Translator.Core.Lexing.TokenParsers
{
    public class NumberParser : ITokenParser
    {
        private readonly Regex regex = new(@"([0-9]*[.])?[0-9]+");
        
        public bool CanParseFrom(Line line, int position)
        {
            var current = line.Value[position];
            var next = position + 1 < line.Length 
                ? line.Value[position + 1]
                : '\0';

            return char.IsDigit(current) || current == '.' && char.IsDigit(next);
        }

        public SyntaxToken Parse(Line line, int position)
        {
            var value = regex
                .Match(line.Value, position)
                .ToString();

            return new SyntaxToken(TokenTypes.Number, value, new TextLocation(line, position));
        }
    }
}