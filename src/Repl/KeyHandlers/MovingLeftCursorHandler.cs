using System;
using Repl.Utils;

namespace Repl.KeyHandlers;

public class MovingLeftCursorHandler : IKeyHandler
{
    public ConsoleKey Key => ConsoleKey.LeftArrow;
    
    public void Handle(ConsoleKeyInfo info, SubmissionDocument submissionDocument)
    {
        submissionDocument.MoveBack();
    }
}