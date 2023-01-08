using System;
using Repl.Utils;

namespace Repl.KeyHandlers;

public class UpArrowHandler : IKeyHandler
{
    public ConsoleKey Key => ConsoleKey.UpArrow;
    
    public void Handle(ConsoleKeyInfo info, SubmissionDocument submissionDocument)
    {
        submissionDocument.MoveToPreviousLine();
    }
}