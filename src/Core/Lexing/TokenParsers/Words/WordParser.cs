using System.Text.RegularExpressions;
using Core.Utils.Text;

namespace Core.Lexing.TokenParsers.Words
{
    public class WordParser : ITokenParser
    {
        private readonly Regex regex = new(@"_*([a-z]|[A-Z]|[А-Я]|[а-я]|[0-9]|_)*");

        public int Priority => 1000;
        
        public SyntaxToken TryParse(Line line, int position)
        {
            var sym = line[position];
            if (!char.IsLetter(sym) && sym != '_')
                return null;
            
            var value = regex
                .Match(line.Value, position)
                .ToString();
            
            var type = value.TryGetKeywordType() ?? TokenTypes.Identifier;
            var location = new Location(line, position, value.Length);

            return new SyntaxToken(type, value, location);
        }
    }
}