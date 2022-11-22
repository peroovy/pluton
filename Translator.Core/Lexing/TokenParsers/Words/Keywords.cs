﻿using System.Collections.Generic;

namespace Translator.Core.Lexing.TokenParsers.Words
{
    internal static class KeywordsExtensions
    {
        private static readonly Dictionary<string, TokenTypes> Keywords = new()
        {
            ["true"] = TokenTypes.TrueKeyword,
            ["false"] = TokenTypes.FalseKeyword,
            ["if"] = TokenTypes.IfKeyword,
            ["else"] = TokenTypes.ElseKeyword
        };

        public static TokenTypes? TryGetKeywordType(this string word) =>
            Keywords.TryGetValue(word, out var type) ? type : null;
    }
}