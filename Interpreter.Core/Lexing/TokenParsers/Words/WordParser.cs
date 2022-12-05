﻿using System.Text.RegularExpressions;
using Interpreter.Core.Text;

namespace Interpreter.Core.Lexing.TokenParsers.Words
{
    public class WordParser : ITokenParser
    {
        private readonly Regex regex = new(@"_*([a-z]|[A-Z]|[А-Я]|[а-я]|[0-9])*");

        public int Priority => 1000;
        
        public SyntaxToken TryParse(Line line, int position)
        {
            var sym = line.Value[position];
            if (!char.IsLetter(sym) && sym != '_')
                return null;
            
            var value = regex
                .Match(line.Value, position)
                .ToString();
            
            var type = value.TryGetKeywordType() ?? TokenTypes.Identifier;

            return new SyntaxToken(type, value, new TextLocation(line, position));
        }
    }
}