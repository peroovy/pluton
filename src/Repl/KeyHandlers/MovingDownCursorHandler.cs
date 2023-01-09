using System;
using Repl.Utils;

namespace Repl.KeyHandlers;

public class MovingDownCursorHandler : IKeyHandler
{
    public ConsoleKey Key => ConsoleKey.DownArrow;

    public ConsoleModifiers Modifiers => default;

    public void Handle(ConsoleKeyInfo info, SubmissionDocument submissionDocument)
    {
        submissionDocument.MoveToNextLine();
    }
}