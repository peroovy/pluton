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
        Semicolon,

        Plus,
        Minus,
        Star,
        Slash,
        OpenParenthesis,
        CloseParenthesis,
        DoubleAmpersand,
        DoublePipe,
        Equals,

        Eof
    }
}