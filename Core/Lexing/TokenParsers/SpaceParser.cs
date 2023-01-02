using Core.Text;

namespace Core.Lexing.TokenParsers
{
    public class SpaceParser : ITokenParser
    {
        public int Priority => 3;
        
        public SyntaxToken TryParse(Line line, int position)
        {
            if (!char.IsWhiteSpace(line[position]))
                return null;
            
            var location = new TextLocation(line, position);
            var value = string.Concat(line.Value.TakeWhileFrom(char.IsWhiteSpace, position));

            return new SyntaxToken(TokenTypes.Space, value, location);
        }
    }
}