using System.Collections.Generic;
using Interpreter.Core.Logging;
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

        public bool CanParseFrom(Line line, int position)
        {
            var actualTerminal = string.Concat(line.Value.TakeFrom(position, 2));

            return terminalsTypes.ContainsKey(actualTerminal);
        }

        public SyntaxToken Parse(Line line, int position, ILogger logger)
        {
            var terminal = string.Concat(line.Value[position], line.Value[position + 1]);
            var type = terminalsTypes[terminal];

            return new SyntaxToken(type, terminal, new TextLocation(line, position));
        }
    }
}