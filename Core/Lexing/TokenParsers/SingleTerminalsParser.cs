using System.Collections.Generic;
using Core.Text;

namespace Core.Lexing.TokenParsers
{
    public class SingleTerminalsParser : ITokenParser
    {
        private readonly Dictionary<char, TokenTypes> terminalsTypes = new()
        {
            ['+'] = TokenTypes.Plus,
            ['-'] = TokenTypes.Minus,
            ['/'] = TokenTypes.Slash,
            ['*'] = TokenTypes.Star,
            ['='] = TokenTypes.Equals,
            ['('] = TokenTypes.OpenParenthesis,
            [')'] = TokenTypes.CloseParenthesis,
            [';'] = TokenTypes.Semicolon,
            ['{'] = TokenTypes.OpenBrace,
            ['}'] = TokenTypes.CloseBrace,
            ['['] = TokenTypes.OpenBracket,
            [']'] = TokenTypes.CloseBracket,
            ['<'] = TokenTypes.LeftArrow,
            ['>'] = TokenTypes.RightArrow,
            [','] = TokenTypes.Comma,
            ['%'] = TokenTypes.Percent,
            ['\0'] = TokenTypes.Eof
        };

        public int Priority => 1;
        
        public SyntaxToken TryParse(Line line, int position)
        {
            if (!terminalsTypes.TryGetValue(line[position], out var type))
                return null;
            
            var terminal = line[position].ToString();
            
            return new SyntaxToken(type, terminal, new TextLocation(line, position));
        }
    }
}