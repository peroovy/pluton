using System;

namespace Repl.KeyHandlers;

public interface IKeyHandler
{
    ConsoleKey Key { get; }

    void Handle(ConsoleKeyInfo info, SubmissionDocument submissionDocument);
}