using System.Text.RegularExpressions;

namespace Translator.Core.Lexing.TokenParsers
{
    public class NumberParser : ITokenParser
    {
        private readonly Regex regex = new Regex(@"([0-9]*[.])?[0-9]+");
        
        public bool IsStartingFrom(string code, int position)
        {
            var current = code[position];
            var next = position + 1 < code.Length 
                ? code[position + 1]
                : '\0';

            return char.IsDigit(current) || current == '.' && char.IsDigit(next);
        }

        public SyntaxToken Parse(string code, int position)
        {
            var value = regex
                .Match(code, position)
                .ToString();

            return new SyntaxToken(TokenTypes.Number, value);
        }
    }
}