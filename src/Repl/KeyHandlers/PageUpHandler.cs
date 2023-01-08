using System;
using Repl.Utils;

namespace Repl.KeyHandlers;

public class PageUpHandler : IKeyHandler
{
    private readonly SubmissionHistory submissionHistory;

    public PageUpHandler(SubmissionHistory submissionHistory)
    {
        this.submissionHistory = submissionHistory;
    }
    
    public ConsoleKey Key => ConsoleKey.PageUp;
    
    public void Handle(ConsoleKeyInfo info, SubmissionDocument submissionDocument)
    {
        if (submissionHistory.IsEmpty)
            return;
        
        submissionDocument.Clear();
        submissionDocument.Insert(submissionHistory.Current);
        submissionHistory.MoveBack();
    }
}