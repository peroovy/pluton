namespace Translator.Core.Lexing
{
    public enum TokenTypes
    {
        Unknown,
        
        Number,
        
        LineSeparator,
        Space,
        
        Plus,
        Minus,
        Star,
        Slash,
        OpenParenthesis,
        CloseParenthesis,
        
        EOF
    }
}