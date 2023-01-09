using System;
using Repl.Utils;

namespace Repl.KeyHandlers;

public class MovingBackSubmissionHistoryHandler : IKeyHandler
{
    private readonly SubmissionHistory submissionHistory;

    public MovingBackSubmissionHistoryHandler(SubmissionHistory submissionHistory)
    {
        this.submissionHistory = submissionHistory;
    }
    
    public ConsoleKey Key => ConsoleKey.PageUp;

    public ConsoleModifiers Modifiers => default;

    public void Handle(ConsoleKeyInfo info, SubmissionDocument submissionDocument)
    {
        if (submissionHistory.IsEmpty)
            return;
        
        submissionDocument.Clear();
        submissionDocument.Insert(submissionHistory.Current);
        submissionHistory.MoveBack();
    }
}