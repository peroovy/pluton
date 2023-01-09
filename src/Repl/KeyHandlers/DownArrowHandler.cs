using System;

namespace Repl.KeyHandlers;

public class DownArrowHandler : IKeyHandler
{
    public ConsoleKey Key => ConsoleKey.DownArrow;
    
    public void Handle(ConsoleKeyInfo info, SubmissionDocument submissionDocument)
    {
        if (submissionDocument.LineIndex + 1 < submissionDocument.LinesCount)
            submissionDocument.LineIndex++;
    }
}