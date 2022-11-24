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
        ForKeyword,

        LineSeparator,
        Space,
        Semicolon,
        Comma,
        ExclamationMark,

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
        LeftArrowEquals,
        RightArrowEquals,
        DoubleEquals,
        ExclamationMarkEquals,
        Percent,

        Eof
    }
}