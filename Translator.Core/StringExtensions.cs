using System;
using System.Collections.Generic;

namespace Translator.Core
{
    internal static class StringExtensions
    {
        public static IEnumerable<char> TakeWhileFrom(this string str, Func<char, bool> predicate, int start)
        {
            while (start < str.Length && predicate(str[start]))
                yield return str[start++];
        }

        public static IEnumerable<char> TakeFrom(this string str, int start, int length)
        {
            while (start < str.Length && length > 0)
            {
                yield return str[start++];
                length--;
            }
        }
    }
}