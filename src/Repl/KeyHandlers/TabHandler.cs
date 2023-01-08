using System;
using Repl.Utils;

namespace Repl.KeyHandlers;

public class TabHandler : IKeyHandler
{
    private const int TabWidth = 4;

    public ConsoleKey Key => ConsoleKey.Tab;

    public void Handle(ConsoleKeyInfo info, SubmissionDocument submissionDocument)
    {
        var tab = new string(' ', TabWidth);
        
        submissionDocument.Insert(tab);
    }
}