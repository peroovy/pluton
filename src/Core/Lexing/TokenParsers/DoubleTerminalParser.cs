using System.Collections.Generic;
using Core.Utils;
using Core.Utils.Diagnostic;
using Core.Utils.Text;

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
            ["/="] = TokenType.SlashEquals,
            ["%="] = TokenType.PercentEquals,
            ["=>"] = TokenType.EqualsRightArrow
        };

        public Priority Priority => Priority.High;
        
        public SyntaxToken TryParse(SourceText text, int position, DiagnosticBag diagnostic)
        {
            var terminal = string.Concat(text.Value.Take(position, 2));
            var location = new Location(text, position, terminal.Length);

            return terminalsTypes.TryGetValue(terminal, out var type) 
                ? new SyntaxToken(type, terminal, location) 
                : null;
        }
    }
}