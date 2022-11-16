using System.Collections.Generic;

namespace Translator.Core.Lexing.TokenParsers.Words
{
    internal static class KeywordsExtensions
    {
        private static readonly Dictionary<string, TokenTypes> Keywords = new Dictionary<string, TokenTypes>
        {
            ["true"] = TokenTypes.TrueKeyword,
            ["false"] = TokenTypes.FalseKeyword
        };

        public static TokenTypes? TryGetKeywordType(this string word) =>
            Keywords.TryGetValue(word, out var type) ? type : (TokenTypes?)null;
    }
}