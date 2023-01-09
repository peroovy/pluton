using System;
using Core.Execution.Objects;
using Core.Utils.Diagnostic;

namespace Repl;

public class Printer : IPrinter
{
    private static readonly string CodeIndent = new(' ', 6);

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

    public void PrintWelcome()
    {
        Console.WriteLine(@"Pluton REPL
Type ""#help"" for more information");
    }

    private static void PrintDiagnostic(Diagnostic diagnostic)
    {
        var sourceText = diagnostic.Location.SourceText;
        var diagnosticSpan = diagnostic.Location.Span;

        var lineIndex = sourceText.GetLineIndex(diagnosticSpan.Start);
        var line = sourceText.Lines[lineIndex];
        var characterIndexInLine = diagnosticSpan.Start - line.Start;
        var level = diagnostic.Level
            .ToString()
            .ToUpper();
        var formattedMessage = $"{level}({lineIndex + 1}, {characterIndexInLine}): {diagnostic.Message}";
        
        Console.ForegroundColor = ErrorFontColor;
        Console.WriteLine(formattedMessage);
        
        Console.ResetColor();
        Console.Write(CodeIndent);
        Console.Write(sourceText.ToString(line.Start, diagnosticSpan.Start - line.Start));

        Console.BackgroundColor = ConsoleColor.DarkRed;
        Console.Write(sourceText.ToString(diagnosticSpan.Start, diagnosticSpan.Length));
        
        Console.ResetColor();
        Console.WriteLine(sourceText.ToString(diagnosticSpan.End, line.End - diagnosticSpan.End + 1));
    }
}