using System;
using Core.Diagnostic;
using Core.Execution.Objects;

namespace Repl;

public class ConsolePrinter : IConsolePrinter
{
    private const int CodeIndent = 6;

    private const ConsoleColor ErrorFontColor = ConsoleColor.DarkRed;

    public void PrintError(string message)
    {
        Console.ForegroundColor = ErrorFontColor;
        Console.WriteLine($"ERROR: {message}");
        Console.ResetColor();
    }

    public void PrintDiagnostic(IDiagnosticBag diagnostic)
    {
        foreach (var log in diagnostic)
            PrintLog(log);
    }

    public void PrintInterpretationResult(Obj value)
    {
        if (value is Null)
            return;
        
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine(value);
        Console.ResetColor();
    }

    private static void PrintLog(Log log)
    {
        var location = log.Location;
        var message = $"{log.Level.ToString().ToUpper()}({location.Line.Number}, {location.Position + 1}): {log.Message}";
        var code = location.Line.Value;
        var trimmedCode = code.TrimStart();
        var highlightStart = location.Position - (code.Length - trimmedCode.Length);
            
        Console.ForegroundColor = ErrorFontColor;
        Console.WriteLine(message);
        
        Console.ResetColor();
        Console.Write(new string(' ', CodeIndent));
        Console.Write(trimmedCode.Substring(0, highlightStart));

        Console.BackgroundColor = ConsoleColor.DarkRed;
        Console.Write(trimmedCode.Substring(highlightStart, log.HighlightCodeLength));
        
        Console.ResetColor();
        Console.WriteLine(trimmedCode.Substring(highlightStart + log.HighlightCodeLength));
    }
}