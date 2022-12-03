namespace Interpreter.Core.Lexing
{
    public enum TokenTypes
    {
        Unknown,
        
        Number,
        Identifier,
        String,

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
        NullKeyword,
        BreakKeyword,
        ContinueKeyword,

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
        OpenBracket,
        CloseBracket,
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