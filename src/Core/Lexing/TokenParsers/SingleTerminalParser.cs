using System.Collections.Generic;
using Core.Utils.Diagnostic;

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
            ['\0'] = TokenType.Eof
        };

        public int Priority => 1;
        
        public SyntaxToken TryParse(Line line, int position, DiagnosticBag diagnostic)
        {
            if (!terminalsTypes.TryGetValue(line[position], out var type))
                return null;
            
            var terminal = line[position].ToString();
            var location = new Location(line, position, terminal.Length);
            
            return new SyntaxToken(type, terminal, location);
        }
    }
}