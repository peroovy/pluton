using System;

namespace Repl;

public class ErrorPrinter : IErrorPrinter
{
    public void Print(string message)
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine($"ERROR: {message}");
        Console.ResetColor();
    }
}