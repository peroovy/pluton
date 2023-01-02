using Core.Text;

namespace Core.Lexing.TokenParsers
{
    public class NextLineParser : ITokenParser
    {
        public int Priority => 0;
        
        public SyntaxToken TryParse(Line line, int position)
        {
            var r = line[position];
            var n = position + 1 < line.Length ? line[position + 1] : '\0';
            var location = new TextLocation(line, position); 

            if (r == '\r' && n == '\n')
                return new SyntaxToken(TokenTypes.NewLine, $"{r}{n}", location);

            if (r == '\n')
                return new SyntaxToken(TokenTypes.NewLine, r.ToString(), location);

            return null;
        }
    }
}