using System;
using Core.Execution.Objects;
using Core.Utils.Diagnostic;
using Repl.Utils;

namespace Repl;

public class ConsolePrinter : IPrinter
{
    private int submissionStartLine;
    private int lastRenderedLineCount;
    
    private static readonly string CodeIndent = new(' ', 6);

    private const ConsoleColor ErrorFontColor = ConsoleColor.DarkRed;
    
    private const string StartPrompt = ">> ";
    private const string ContinuePrompt = ".. ";
    private const int PromptLength = 3;

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

    public void FreezeDocumentStartLine()
    {
        submissionStartLine = Console.CursorTop;
    }

    public void PrintSubmission(SubmissionDocument document)
    {
        Console.CursorVisible = false;
        var lineCount = 0;

        foreach (var line in document)
        {
            Console.SetCursorPosition(0, submissionStartLine + lineCount);

            var prompt = lineCount == 0 ? StartPrompt : ContinuePrompt;
            var textLength = prompt.Length + line.Length;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(prompt);
            Console.ResetColor();

            Console.Write(line);
            Console.Write(new string(' ', Console.WindowWidth - textLength - 1));

            lineCount++;
        }
        
        var blankLinesCount = lastRenderedLineCount - lineCount;
        if (blankLinesCount > 0)
        {
            var blankLine = new string(' ', Console.WindowWidth);
            for (var i = 0; i < blankLinesCount; i++)
            {
                Console.SetCursorPosition(0, submissionStartLine + lineCount + i);
                Console.WriteLine(blankLine);
            }
        }
        lastRenderedLineCount = lineCount;
        
        Console.CursorVisible = true;
    }

    public void SetCursorToDocumentEnd(SubmissionDocument document)
    {
        Console.SetCursorPosition(
            PromptLength + document.CharacterIndex,
            submissionStartLine + document.LineIndex
        );
    }

    public void SetCursorAfterDocument(SubmissionDocument document)
    {
        Console.SetCursorPosition(0, submissionStartLine + document.LineCount);
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