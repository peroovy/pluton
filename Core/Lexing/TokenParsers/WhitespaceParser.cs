using Core.Text;

namespace Core.Lexing.TokenParsers
{
    public class WhitespaceParser : ITokenParser
    {
        public int Priority => 3;
        
        public SyntaxToken TryParse(Line line, int position)
        {
            if (!char.IsWhiteSpace(line.Value[position]))
                return null;
            
            var location = new TextLocation(line, position);
            
            if (line.Value[position] == '\n')
                return new SyntaxToken(TokenTypes.LineSeparator,"\n", location);

            var value = string.Concat(
                line.Value.TakeWhileFrom(sym => char.IsWhiteSpace(sym) && sym != '\n', position)
            );

            return new SyntaxToken(TokenTypes.Space, value, location);
        }
    }
}