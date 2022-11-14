namespace Translator.Core.Lexing
{
    public class SyntaxToken
    {
        public SyntaxToken(TokenTypes type, string value)
        {
            Type = type;
            Value = value;
        }
        
        public TokenTypes Type { get; }
        
        public string Value { get; }

        public int Lenght => Value.Length;

        public override string ToString() => $"{Type}: {Value}";
    }
}