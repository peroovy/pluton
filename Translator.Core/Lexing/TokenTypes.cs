namespace Translator.Core.Lexing
{
    public enum TokenTypes
    {
        Unknown,
        
        Number,
        Identifier,
        
        TrueKeyword,
        FalseKeyword,

        LineSeparator,
        Space,
        
        Plus,
        Minus,
        Star,
        Slash,
        OpenParenthesis,
        CloseParenthesis,
        DoubleAmpersand,
        DoublePipe,
        Equals,

        EOF
    }
}