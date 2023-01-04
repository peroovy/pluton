using System;

namespace Repl.KeyHandlers;

public class RightArrowHandler : IKeyHandler
{
    public ConsoleKey Key => ConsoleKey.RightArrow;
    
    public void Handle(ConsoleKeyInfo info, SubmissionDocument submissionDocument)
    {
        if (submissionDocument.CharacterLeftIndex + 1 < submissionDocument.CharactersCount)
            submissionDocument.CharacterLeftIndex++;
    }
}