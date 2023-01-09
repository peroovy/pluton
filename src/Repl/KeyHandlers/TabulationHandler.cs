using System;
using Repl.Utils;

namespace Repl.KeyHandlers;

public class TabulationHandler : IKeyHandler
{
    private const int TabWidth = 4;

    public ConsoleKey Key => ConsoleKey.Tab;

    public ConsoleModifiers Modifiers => default;

    public void Handle(ConsoleKeyInfo info, SubmissionDocument submissionDocument)
    {
        var tab = new string(' ', TabWidth);
        
        submissionDocument.Insert(tab);
    }
}