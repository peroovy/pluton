using System.Text.RegularExpressions;
using Translator.Core.Logging;
using Translator.Core.Text;

namespace Translator.Core.Lexing.TokenParsers.Words
{
    public class WordParser : ITokenParser
    {
        private readonly Regex regex = new Regex(@"_*([A-z]|[А-я]|[0-9])*");
        
        public bool CanParseFrom(Line line, int position)
        {
            var current = line.Value[position];

            return char.IsLetter(current) || current == '_';
        }

        public SyntaxToken Parse(Line line, int position)
        {
            var value = regex
                .Match(line.Value, position)
                .ToString();

            var type = value.TryGetKeywordType() ?? TokenTypes.Identifier;

            return new SyntaxToken(type, value, new TextLocation(line, position));
        }
    }
}