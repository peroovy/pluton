using System;
using Core.Execution.Objects;
using Core.Utils.Diagnostic;

namespace Repl;

public class Printer : IPrinter
{
    private static readonly string CodeIndent = new string(' ', 6);

    private const ConsoleColor ErrorFontColor = ConsoleColor.DarkRed;

    public void PrintError(string message)
    {
        Console.ForegroundColor = ErrorFontColor;
        Console.WriteLine($"ERROR: {message}");
        Console.ResetColor();
    }

    public void PrintDiagnostic(DiagnosticBag diagnosticBag)
    {
        foreach (var diagnostic in diagnosticBag)
            PrintDiagnostic(diagnostic);
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

    private static void PrintDiagnostic(Diagnostic diagnostic)
    {
        var sourceText = diagnostic.Location.SourceText;
        var location = diagnostic.Location;

        var lineIndex = sourceText.GetLineIndex(location.Start);
        var line = sourceText.Lines[lineIndex];
        var characterIndex = location.Start - line.Start;
        var level = diagnostic.Level
            .ToString()
            .ToUpper();
        var formattedMessage = $"{level}({lineIndex + 1}, {characterIndex}): {diagnostic.Message}";
        
        Console.ForegroundColor = ErrorFontColor;
        Console.WriteLine(formattedMessage);
        
        Console.ResetColor();
        Console.Write(CodeIndent);
        Console.Write(line.ToString(0, characterIndex));

        Console.BackgroundColor = ConsoleColor.DarkRed;
        Console.Write(line.ToString(characterIndex, location.Length));
        
        Console.ResetColor();
        Console.WriteLine(line.ToString(characterIndex + location.Length));
    }
}