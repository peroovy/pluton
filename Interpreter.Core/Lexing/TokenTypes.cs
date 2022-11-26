namespace Interpreter.Core.Lexing
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
        DefKeyword,
        ReturnKeyword,
        AndKeyword,
        OrKeyword,

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
        Equals,
        LeftArrow,
        RightArrow,
        LeftArrowEquals,
        RightArrowEquals,
        DoubleEquals,
        PlusEquals,
        MinusEquals,
        StarEquals,
        SlashEquals,
        ExclamationMarkEquals,
        Percent,

        Eof
    }
}