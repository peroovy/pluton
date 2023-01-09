using System;
using Repl.Utils;

namespace Repl.KeyHandlers;

public class MovingNextSubmissionHistoryHandler : IKeyHandler
{
    private readonly SubmissionHistory submissionHistory;

    public MovingNextSubmissionHistoryHandler(SubmissionHistory submissionHistory)
    {
        this.submissionHistory = submissionHistory;
    }
    
    public ConsoleKey Key => ConsoleKey.PageDown;

    public ConsoleModifiers Modifiers => default;

    public void Handle(ConsoleKeyInfo info, SubmissionDocument submissionDocument)
    {
        if (submissionHistory.IsEmpty)
            return;
        
        submissionDocument.Clear();
        submissionDocument.Insert(submissionHistory.Current);
        submissionHistory.MoveNext();
    }
}