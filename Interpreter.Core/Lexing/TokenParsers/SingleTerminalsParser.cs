using System.Collections.Generic;
using Interpreter.Core.Logging;
using Interpreter.Core.Text;

namespace Interpreter.Core.Lexing.TokenParsers
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
            ['!'] = TokenTypes.ExclamationMark,
            ['%'] = TokenTypes.Percent,
            ['\0'] = TokenTypes.Eof
        };

        public int Priority => 1;
        
        public bool CanParseFrom(Line line, int position) => terminalsTypes.ContainsKey(line.Value[position]);

        public SyntaxToken Parse(Line line, int position, ILogger logger)
        {
            var terminal = line.Value[position];
            var type = terminalsTypes[terminal];
            
            return new SyntaxToken(type, terminal.ToString(), new TextLocation(line, position));
        }
    }
}