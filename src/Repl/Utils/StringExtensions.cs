namespace Repl.Utils;

public static class StringExtensions
{
    public static string Capitalize(this string str)
    {
        return str.Length == 0
            ? string.Empty
            : char.ToUpper(str[0]) + str.Substring(1);
    }
}