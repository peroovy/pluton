using System;
using Repl.Utils;

namespace Repl.KeyHandlers;

public class DeletingEnteredSubmissionHandler : IKeyHandler
{
    public ConsoleKey Key => ConsoleKey.Backspace;

    public ConsoleModifiers Modifiers => ConsoleModifiers.Control;

    public void Handle(ConsoleKeyInfo info, SubmissionDocument submissionDocument)
    {
        if (!submissionDocument.IsEmpty)
            submissionDocument.Clear();
    }
}