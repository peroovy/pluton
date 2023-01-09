using System;

namespace Repl.KeyHandlers;

public class UpArrowHandler : IKeyHandler
{
    public ConsoleKey Key => ConsoleKey.UpArrow;
    
    public void Handle(ConsoleKeyInfo info, SubmissionDocument submissionDocument)
    {
        if (submissionDocument.LineIndex > 0)
            submissionDocument.LineIndex--;
    }
}