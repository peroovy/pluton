using System;

namespace Repl;

public class Repl
{
    private int cursorTop;
    
    private const string StartPrompt = ">> ";
    private const string ContinuePrompt = ".. ";
    private const int PromptLength = 3;
    
    public void Run()
    {
        var submissionDocument = new SubmissionDocument();
        submissionDocument.OnChanged += Render;

        submissionDocument.InsertEmptyLine();
        UpdateCursor(submissionDocument);
        while (true)
        {
            var info = Console.ReadKey(true);
            switch (info.Key)
            {
                case ConsoleKey.Enter:
                    submissionDocument.InsertEmptyLine();
                    break;

                case ConsoleKey.Backspace:
                    submissionDocument.DeleteCharacter();
                    break;

                case ConsoleKey.LeftArrow:
                    submissionDocument.CharacterIndex--;
                    break;
                
                case ConsoleKey.RightArrow:
                    submissionDocument.CharacterIndex++;
                    break;
                
                case ConsoleKey.UpArrow:
                    submissionDocument.LineIndex--;
                    break;
                
                case ConsoleKey.DownArrow:
                    submissionDocument.LineIndex++;
                    break;
                    
                default:
                {
                    if (info.KeyChar >= ' ')
                        submissionDocument.Insert(info.KeyChar);
                    break;
                }
            }
            
            UpdateCursor(submissionDocument);
        }
    }
    
    private void Render(SubmissionDocument submissionDocument)
    {
        Console.CursorVisible = false;
        var lineCount = 0;

        foreach (var line in submissionDocument)
        {
            Console.SetCursorPosition(0, cursorTop + lineCount);

            var prompt = lineCount == 0 ? StartPrompt : ContinuePrompt;
            var textLength = prompt.Length + line.Length;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(prompt);
            Console.ResetColor();

            Console.Write(line);
            Console.Write(new string(' ', Console.WindowWidth - textLength - 1));

            lineCount++;
        }
        
        Console.CursorVisible = true;
    }

    private void UpdateCursor(SubmissionDocument submissionDocument)
    {
        Console.SetCursorPosition(PromptLength + submissionDocument.CharacterIndex + 1,
            cursorTop + submissionDocument.LineIndex);
    }
}