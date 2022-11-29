using System.Text.RegularExpressions;
using Interpreter.Core.Logging;
using Interpreter.Core.Text;

namespace Interpreter.Core.Lexing.TokenParsers.Words
{
    public class WordParser : ITokenParser
    {
        private readonly Regex regex = new(@"_*([A-z]|[А-я]|[0-9])*");

        public int Priority => 1000;

        public bool CanParseFrom(Line line, int position)
        {
            var current = line.Value[position];

            return char.IsLetter(current) || current == '_';
        }

        public SyntaxToken Parse(Line line, int position, ILogger logger)
        {
            var value = regex
                .Match(line.Value, position)
                .ToString();

            var type = value.TryGetKeywordType() ?? TokenTypes.Identifier;

            return new SyntaxToken(type, value, new TextLocation(line, position));
        }
    }
}