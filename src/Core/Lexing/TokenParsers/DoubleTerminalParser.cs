using System.Collections.Generic;
using Core.Utils;
using Core.Utils.Diagnostic;

namespace Core.Lexing.TokenParsers
{
    public class DoubleTerminalParser : ITokenParser
    {
        private readonly Dictionary<string, TokenType> terminalsTypes = new()
        {
            ["<="] = TokenType.LeftArrowEquals,
            [">="] = TokenType.RightArrowEquals,
            ["=="] = TokenType.DoubleEquals,
            ["!="] = TokenType.ExclamationMarkEquals,
            ["+="] = TokenType.PlusEquals,
            ["-="] = TokenType.MinusEquals,
            ["*="] = TokenType.StarEquals,
            ["/="] = TokenType.SlashEquals
        };

        public Priority Priority => Priority.High;
        
        public SyntaxToken TryParse(Line line, int position, DiagnosticBag diagnostic)
        {
            var terminal = string.Concat(line.Value.Take(position, 2));
            var location = new Location(line, position, terminal.Length);

            return terminalsTypes.TryGetValue(terminal, out var type) 
                ? new SyntaxToken(type, terminal, location) 
                : null;
        }
    }
}