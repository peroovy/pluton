using System.Collections.Generic;
using Interpreter.Core.Text;

namespace Interpreter.Core.Lexing.TokenParsers
{
    public class DoubleTerminalsParser : ITokenParser
    {
        private readonly Dictionary<string, TokenTypes> terminalsTypes = new()
        {
            ["<="] = TokenTypes.LeftArrowEquals,
            [">="] = TokenTypes.RightArrowEquals,
            ["=="] = TokenTypes.DoubleEquals,
            ["!="] = TokenTypes.ExclamationMarkEquals,
            ["+="] = TokenTypes.PlusEquals,
            ["-="] = TokenTypes.MinusEquals,
            ["*="] = TokenTypes.StarEquals,
            ["/="] = TokenTypes.SlashEquals
        };

        public int Priority => 0;
        
        public SyntaxToken TryParse(Line line, int position)
        {
            var terminal = string.Concat(line.Value.TakeFrom(position, 2));

            return terminalsTypes.TryGetValue(terminal, out var type) 
                ? new SyntaxToken(type, terminal, new TextLocation(line, position)) 
                : null;
        }
    }
}