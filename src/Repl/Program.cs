using System.Globalization;

namespace Repl;

public static class Program
{
    public static void Main(string[] args)
    {
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

        var repl = new Repl();
        repl.Run();
    }
}