namespace Interpreter.Core.Lexing
{
    public class SyntaxToken
    {
        public SyntaxToken(TokenTypes type, string text, TextLocation location)
        {
            Type = type;
            Text = text;
            Location = location;
        }
        
        public TokenTypes Type { get; }
        
        public string Text { get; }
        
        public TextLocation Location { get; }

        public int Length => Text.Length;

        public override string ToString() => $"{Type}: {Text}";
    }
}