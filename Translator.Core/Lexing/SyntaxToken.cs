namespace Translator.Core.Lexing
{
    public class SyntaxToken
    {
        public SyntaxToken(TokenTypes type, string value, TextLocation location)
        {
            Type = type;
            Value = value;
            Location = location;
        }
        
        public TokenTypes Type { get; }
        
        public string Value { get; }
        
        public TextLocation Location { get; }

        public int Length => Value.Length;

        public override string ToString() => $"{Type}: {Value}";
    }
}