using Core.Utils;
using Core.Utils.Text;

namespace Core.Lexing.TokenParsers
{
    public class SpaceParser : ITokenParser
    {
        public int Priority => 3;
        
        public SyntaxToken TryParse(Line line, int position)
        {
            if (!char.IsWhiteSpace(line[position]))
                return null;
            
            var value = string.Concat(line.Value.TakeWhile(char.IsWhiteSpace, position));
            var location = new Location(line, position, value.Length);

            return new SyntaxToken(TokenTypes.Space, value, location);
        }
    }
}