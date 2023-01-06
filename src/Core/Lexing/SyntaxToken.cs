using Core.Utils.Diagnostic;
using Core.Utils.Text;

namespace Core.Lexing
{
    public class SyntaxToken
    {
        public SyntaxToken(TokenType type, string text, Location location)
        {
            Type = type;
            Text = text;
            Location = location;
        }

        public TokenType Type { get; }
        
        public string Text { get; }
        
        public Location Location { get; }

        public override string ToString() => $"{Type}: {Text}";
    }
}