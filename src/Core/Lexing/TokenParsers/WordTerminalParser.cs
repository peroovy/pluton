using System.Collections.Generic;
using System.Text.RegularExpressions;
using Core.Utils.Diagnostic;

namespace Core.Lexing.TokenParsers
{
    public class WordTerminalParser : ITokenParser
    {
        private readonly Regex regex = new(@"_*([a-z]|[A-Z]|[А-Я]|[а-я]|[0-9]|_)*");
        
        private static readonly Dictionary<string, TokenType> Keywords = new()
        {
            ["true"] = TokenType.TrueKeyword,
            ["false"] = TokenType.FalseKeyword,
            ["if"] = TokenType.IfKeyword,
            ["else"] = TokenType.ElseKeyword,
            ["while"] = TokenType.WhileKeyword,
            ["for"] = TokenType.ForKeyword,
            ["def"] = TokenType.DefKeyword,
            ["return"] = TokenType.ReturnKeyword,
            ["and"] = TokenType.AndKeyword,
            ["or"] = TokenType.OrKeyword,
            ["not"] = TokenType.NotKeyword,
            ["null"] = TokenType.NullKeyword,
            ["break"] = TokenType.BreakKeyword,
            ["continue"] = TokenType.ContinueKeyword
        };

        public Priority Priority => Priority.Low;
        
        public SyntaxToken TryParse(Line line, int position, DiagnosticBag diagnostic)
        {
            var sym = line[position];
            if (!char.IsLetter(sym) && sym != '_')
                return null;
            
            var value = regex
                .Match(line.Value, position)
                .ToString();
            
            var type = Keywords.TryGetValue(value, out var keywordType) 
                ? keywordType
                : TokenType.Identifier;
            var location = new Location(line, position, value.Length);

            return new SyntaxToken(type, value, location);
        }
    }
}