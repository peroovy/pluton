namespace Core.Lexing
{
    public class SyntaxToken
    {
        public SyntaxToken(TokenTypes type, string text, TextLocation location)
        {
            Type = type;
            Text = text;
            Location = location;
            Length = text?.Length ?? 0;
        }

        public SyntaxToken(TokenTypes type, string text, TextLocation location, int length)
            : this(type, text, location)
        {
            Length = length;
        }
        
        public TokenTypes Type { get; }
        
        public string Text { get; }
        
        public TextLocation Location { get; }

        public int Length { get; }

        public override string ToString() => $"{Type}: {Text}";
    }
}