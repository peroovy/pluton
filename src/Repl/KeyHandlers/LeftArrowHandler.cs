using System;

namespace Repl.KeyHandlers;

public class LeftArrowHandler : IKeyHandler
{
    public ConsoleKey Key => ConsoleKey.LeftArrow;
    
    public void Handle(ConsoleKeyInfo info, SubmissionDocument submissionDocument)
    {
        if (submissionDocument.CharacterLeftIndex > -1)
            submissionDocument.CharacterLeftIndex--;
    }
}