using System;
using Repl.Utils;

namespace Repl.KeyHandlers;

public interface IKeyHandler
{
    ConsoleKey Key { get; }
    
    ConsoleModifiers Modifiers { get; }

    void Handle(ConsoleKeyInfo info, SubmissionDocument submissionDocument);
}