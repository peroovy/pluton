using System;
using Repl.Utils;

namespace Repl.KeyHandlers;

public class RemovingCharacterRightHandler : IKeyHandler
{
    public ConsoleKey Key => ConsoleKey.Delete;

    public ConsoleModifiers Modifiers => default;
    
    public void Handle(ConsoleKeyInfo info, SubmissionDocument submissionDocument)
    {
        submissionDocument.RemoveRight();
    }
}