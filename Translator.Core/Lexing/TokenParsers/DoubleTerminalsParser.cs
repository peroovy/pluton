using System.Collections.Generic;
using Translator.Core.Logging;
using Translator.Core.Text;

namespace Translator.Core.Lexing.TokenParsers
{
    public class DoubleTerminalsParser : ITokenParser
    {
        private readonly Dictionary<string, TokenTypes> terminalsTypes =
            new Dictionary<string, TokenTypes>
            {
                ["&&"] = TokenTypes.DoubleAmpersand,
                ["||"] = TokenTypes.DoublePipe
            };

        public bool CanParseFrom(Line line, int position)
        {
            var actualTerminal = string.Concat(line.Value.TakeFrom(position, 2));

            return terminalsTypes.ContainsKey(actualTerminal);
        }

        public SyntaxToken Parse(Line line, int position)
        {
            var terminal = string.Concat(line.Value[position], line.Value[position + 1]);
            var type = terminalsTypes[terminal];

            return new SyntaxToken(type, terminal, new TextLocation(line, position));
        }
    }
}