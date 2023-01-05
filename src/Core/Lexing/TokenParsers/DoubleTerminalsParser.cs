using System.Collections.Generic;
using Core.Utils;
using Core.Utils.Text;

namespace Core.Lexing.TokenParsers
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
            var terminal = string.Concat(line.Value.Take(position, 2));
            var location = new Location(line, position, terminal.Length);

            return terminalsTypes.TryGetValue(terminal, out var type) 
                ? new SyntaxToken(type, terminal, location) 
                : null;
        }
    }
}