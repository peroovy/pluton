using System.Collections.Generic;
using Core.Utils.Diagnostic;
using Core.Utils.Text;

namespace Core.Lexing.TokenParsers
{
    public class SingleTerminalParser : ITokenParser
    {
        private readonly Dictionary<char, TokenType> terminalsTypes = new()
        {
            ['+'] = TokenType.Plus,
            ['-'] = TokenType.Minus,
            ['/'] = TokenType.Slash,
            ['*'] = TokenType.Star,
            ['='] = TokenType.Equals,
            ['('] = TokenType.OpenParenthesis,
            [')'] = TokenType.CloseParenthesis,
            [';'] = TokenType.Semicolon,
            ['{'] = TokenType.OpenBrace,
            ['}'] = TokenType.CloseBrace,
            ['['] = TokenType.OpenBracket,
            [']'] = TokenType.CloseBracket,
            ['<'] = TokenType.LeftArrow,
            ['>'] = TokenType.RightArrow,
            [','] = TokenType.Comma,
            ['%'] = TokenType.Percent,
            ['?'] = TokenType.QuestionMark,
            [':'] = TokenType.Colon,
            ['.'] = TokenType.Dot,
            ['\0'] = TokenType.Eof
        };

        public Priority Priority => Priority.Low;
        
        public SyntaxToken TryParse(SourceText text, int position, DiagnosticBag diagnostic)
        {
            if (!terminalsTypes.TryGetValue(text[position], out var type))
                return null;
            
            var terminal = text[position].ToString();
            var location = new Location(text, position, terminal.Length);
            
            return new SyntaxToken(type, terminal, location);
        }
    }
}