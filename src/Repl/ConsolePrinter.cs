using System;
using System.Collections.Generic;
using Core.Execution.DataModel.Objects;
using Core.Utils.Diagnostic;
using Repl.MetaCommands;
using Repl.Utils;

namespace Repl;

public class ConsolePrinter : IPrinter
{
    private readonly Lazy<IEnumerable<IMetaCommand>> metaCommands;
    
    private static readonly string CodeIndent = new(' ', 6);

    private const ConsoleColor ErrorFontColor = ConsoleColor.DarkRed;

    public ConsolePrinter(Lazy<IEnumerable<IMetaCommand>> metaCommands)
    {
        this.metaCommands = metaCommands;
    }

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
        Console.WriteLine();
        Console.WriteLine("Welcome to Pluton REPL");
        Console.WriteLine("Type \"#help\" for more information");
        Console.WriteLine();
    }

    public void PrintHelp()
    {
        Console.WriteLine(@"You can use:

Enter to execute a submission or go to the next line with hyphenation.
Shift+Enter to force start execution.
Ctrl+Enter to go to the next line.
Arrows (↑ and ↓) to navigate within a multi-line submission.
PageUp and PageDown to navigate through submission history.
Backspace to remove the character on the left.
Delete to remove the character on the right.
Ctrl+Backspace to delete the entered submission.");
        
        Console.WriteLine();
        Console.WriteLine();
        
        Console.WriteLine("Meta-commands available:");
        Console.WriteLine();
        foreach (var metaCommand in metaCommands.Value)
            Console.WriteLine($"#{metaCommand.Name} - {metaCommand.Description.Capitalize()}");
        
        Console.WriteLine();
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