using System;
using Repl.Utils;

namespace Repl.KeyHandlers;

public class MovingRightCursorHandler : IKeyHandler
{
    public ConsoleKey Key => ConsoleKey.RightArrow;

    public ConsoleModifiers Modifiers => default;

    public void Handle(ConsoleKeyInfo info, SubmissionDocument submissionDocument)
    {
        submissionDocument.MoveNext();
    }
}