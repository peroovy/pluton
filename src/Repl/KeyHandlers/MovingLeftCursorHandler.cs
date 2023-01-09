using System;
using Repl.Utils;

namespace Repl.KeyHandlers;

public class MovingLeftCursorHandler : IKeyHandler
{
    public ConsoleKey Key => ConsoleKey.LeftArrow;

    public ConsoleModifiers Modifiers => default;

    public void Handle(ConsoleKeyInfo info, SubmissionDocument submissionDocument)
    {
        submissionDocument.MoveBack();
    }
}