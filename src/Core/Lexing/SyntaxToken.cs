using Core.Utils.Text;

namespace Core.Lexing
{
    public class SyntaxToken
    {
        public SyntaxToken(TokenTypes type, string text, Location location)
        {
            Type = type;
            Text = text;
            Location = location;
        }

        public TokenTypes Type { get; }
        
        public string Text { get; }
        
        public Location Location { get; }

        public override string ToString() => $"{Type}: {Text}";
    }
}