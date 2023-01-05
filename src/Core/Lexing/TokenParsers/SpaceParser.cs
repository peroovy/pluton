using Core.Utils;
using Core.Utils.Diagnostic;

namespace Core.Lexing.TokenParsers
{
    public class SpaceParser : ITokenParser
    {
        public Priority Priority => Priority.Low;
        
        public SyntaxToken TryParse(Line line, int position, DiagnosticBag diagnostic)
        {
            if (!char.IsWhiteSpace(line[position]))
                return null;
            
            var value = string.Concat(line.Value.TakeWhile(char.IsWhiteSpace, position));
            var location = new Location(line, position, value.Length);

            return new SyntaxToken(TokenType.Space, value, location);
        }
    }
}