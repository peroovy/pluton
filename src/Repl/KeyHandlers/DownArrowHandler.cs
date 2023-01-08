using System;
using Repl.Utils;

namespace Repl.KeyHandlers;

public class DownArrowHandler : IKeyHandler
{
    public ConsoleKey Key => ConsoleKey.DownArrow;
    
    public void Handle(ConsoleKeyInfo info, SubmissionDocument submissionDocument)
    {
        submissionDocument.MoveToNextLine();
    }
}