using System.Collections.Generic;
using Translator.Core.Logging;
using Translator.Core.Text;

namespace Translator.Core.Lexing.TokenParsers
{
    public class SingleTerminalsParser : ITokenParser
    {
        private readonly Dictionary<char, TokenTypes> terminalsTypes = new Dictionary<char, TokenTypes>
        {
            ['+'] = TokenTypes.Plus,
            ['-'] = TokenTypes.Minus,
            ['/'] = TokenTypes.Slash,
            ['*'] = TokenTypes.Star,
            ['='] = TokenTypes.Equals,
            ['('] = TokenTypes.OpenParenthesis,
            [')'] = TokenTypes.CloseParenthesis,
            [';'] = TokenTypes.Semicolon,
            ['\0'] = TokenTypes.Eof
        };

        public bool CanParseFrom(Line line, int position) => terminalsTypes.ContainsKey(line.Value[position]);

        public SyntaxToken Parse(Line line, int position)
        {
            var terminal = line.Value[position];
            var type = terminalsTypes[terminal];
            
            return new SyntaxToken(type, terminal.ToString(), new TextLocation(line, position));
        }
    }
}