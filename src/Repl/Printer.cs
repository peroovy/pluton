using System;
using Core.Execution.Objects;
using Core.Utils.Diagnostic;

namespace Repl;

public class Printer : IPrinter
{
    private const int CodeIndent = 6;

    private const ConsoleColor ErrorFontColor = ConsoleColor.DarkRed;

    public void PrintError(string message)
    {
        Console.ForegroundColor = ErrorFontColor;
        Console.WriteLine($"ERROR: {message}");
        Console.ResetColor();
    }

    public void PrintDiagnostic(DiagnosticBag diagnostic)
    {
        foreach (var log in diagnostic)
            PrintLog(log);
    }

    public void PrintResult(Obj value)
    {
        if (value is Null)
            return;
        
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine(value.AsDebugString);
        Console.ResetColor();
    }

    public void PrintBlankLine()
    {
        Console.WriteLine();
    }

    private static void PrintLog(Log log)
    {
        var formattedMessage = GetFormattedMessageFrom(log);
        
        var codeLine = log.Location.Line.Value;
        var trimmedCode = codeLine.TrimStart();
        var highlightStart = log.Location.Start - (codeLine.Length - trimmedCode.Length);
            
        Console.ForegroundColor = ErrorFontColor;
        Console.WriteLine(formattedMessage);
        
        Console.ResetColor();
        Console.Write(new string(' ', CodeIndent));
        Console.Write(trimmedCode.Substring(0, highlightStart));

        Console.BackgroundColor = ConsoleColor.DarkRed;
        Console.Write(trimmedCode.Substring(highlightStart, log.Location.Length));
        
        Console.ResetColor();
        Console.WriteLine(trimmedCode.Substring(highlightStart + log.Location.Length));
    }

    private static string GetFormattedMessageFrom(Log log)
    {
        var location = log.Location;
        var level = log.Level
            .ToString()
            .ToUpper();
        
        return $"{level}({location.Line.Index}, {location.Start + 1}): {log.Message}";
    }
}