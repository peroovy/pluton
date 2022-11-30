using System.Collections.Generic;

namespace Interpreter.Core.Lexing.TokenParsers.Words
{
    internal static class KeywordsExtensions
    {
        private static readonly Dictionary<string, TokenTypes> Keywords = new()
        {
            ["true"] = TokenTypes.TrueKeyword,
            ["false"] = TokenTypes.FalseKeyword,
            ["if"] = TokenTypes.IfKeyword,
            ["else"] = TokenTypes.ElseKeyword,
            ["while"] = TokenTypes.WhileKeyword,
            ["for"] = TokenTypes.ForKeyword,
            ["def"] = TokenTypes.DefKeyword,
            ["return"] = TokenTypes.ReturnKeyword,
            ["and"] = TokenTypes.AndKeyword,
            ["or"] = TokenTypes.OrKeyword,
            ["null"] = TokenTypes.NullKeyword
        };

        public static TokenTypes? TryGetKeywordType(this string word) =>
            Keywords.TryGetValue(word, out var type) ? type : null;
    }
}