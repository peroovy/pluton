using Core.Utils;
using Core.Utils.Diagnostic;
using Core.Utils.Text;

namespace Core.Lexing.TokenParsers
{
    public class SpaceParser : ITokenParser
    {
        public Priority Priority => Priority.Low;
        
        public SyntaxToken TryParse(SourceText text, int position, DiagnosticBag diagnostic)
        {
            if (!char.IsWhiteSpace(text[position]))
                return null;
            
            var value = string.Concat(text.Value.TakeWhile(char.IsWhiteSpace, position));
            var location = new Location(text, position, value.Length);

            return new SyntaxToken(TokenType.Space, value, location);
        }
    }
}