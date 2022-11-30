using System;
using System.Text.RegularExpressions;
using Castle.Core.Internal;
using Interpreter.Core.Logging;
using Interpreter.Core.Text;

namespace Interpreter.Core.Lexing.TokenParsers.Words
{
    public class WordParser : ITokenParser
    {
        private readonly Regex regex = new(@"_*([a-z]|[A-Z]|[А-Я]|[а-я]|[0-9])*");

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

            if (value.IsNullOrEmpty())
                throw new ArgumentException("Unknown word");

            var type = value.TryGetKeywordType() ?? TokenTypes.Identifier;

            return new SyntaxToken(type, value, new TextLocation(line, position));
        }
    }
}