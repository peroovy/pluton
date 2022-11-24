namespace Translator.Core.Lexing
{
    public enum TokenTypes
    {
        Unknown,
        
        Number,
        Identifier,
        
        TrueKeyword,
        FalseKeyword,
        IfKeyword,
        ElseKeyword,
        WhileKeyword,

        LineSeparator,
        Space,
        Semicolon,

        Plus,
        Minus,
        Star,
        Slash,
        OpenParenthesis,
        CloseParenthesis,
        OpenBrace,
        CloseBrace,
        DoubleAmpersand,
        DoublePipe,
        Equals,
        LeftArrow,
        RightArrow,

        Eof
    }
}