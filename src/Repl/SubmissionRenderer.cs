using System;
using Repl.Utils;

namespace Repl;

public class SubmissionRenderer
{
    private readonly SubmissionDocument document;
    private readonly int startLine;

    private int lastRenderedLineCount;
    private bool isCompleted;
    
    private const string StartPrompt = ">> ";
    private const string ContinuePrompt = ".. ";
    private const int PromptLength = 3;
    
    public SubmissionRenderer(SubmissionDocument document)
    {
        this.document = document;
        startLine = Console.CursorTop;
    }
    
    public void Render()
    {
        if (isCompleted)
            throw new InvalidOperationException("The document is completed");
        
        Console.CursorVisible = false;
        var lineCount = 0;

        foreach (var line in document)
        {
            Console.SetCursorPosition(0, startLine + lineCount);

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
                Console.SetCursorPosition(0, startLine + lineCount + i);
                Console.WriteLine(blankLine);
            }
        }
        lastRenderedLineCount = lineCount;
        
        Console.CursorVisible = true;
        Console.SetCursorPosition(PromptLength + document.CharacterIndex,startLine + document.LineIndex);
    }

    public void Complete()
    {
        isCompleted = true;
        Console.SetCursorPosition(0, startLine + document.LineCount);
    }
}