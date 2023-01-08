using System;
using Repl.Utils;

namespace Repl.KeyHandlers;

public class MovingUpCursorHandler : IKeyHandler
{
    public ConsoleKey Key => ConsoleKey.UpArrow;

    public ConsoleModifiers Modifiers => default;

    public void Handle(ConsoleKeyInfo info, SubmissionDocument submissionDocument)
    {
        submissionDocument.MoveToPreviousLine();
    }
}