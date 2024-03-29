﻿using System.Text.RegularExpressions;
using Core.Utils.Diagnostic;
using Core.Utils.Text;

namespace Core.Lexing.TokenParsers
{
    public class NumberParser : ITokenParser
    {
        private readonly Regex regex = new(@"([0-9]*[.])?[0-9]+");

        public Priority Priority => Priority.Low;
        
        public SyntaxToken TryParse(SourceText text, int position, DiagnosticBag diagnostic)
        {
            var current = text[position];
            var next = position + 1 < text.Length ? text[position + 1] : '\0';

            if (!(char.IsDigit(current) || current == '.' && char.IsDigit(next)))
                return null;
            
            var value = regex
                .Match(text.Value, position)
                .ToString();

            var location = new Location(text,position, value.Length);
            return new SyntaxToken(TokenType.Number, value, location);
        }
    }
}