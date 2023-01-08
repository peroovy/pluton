using System.Collections.Generic;

namespace Repl.Utils;

public class SubmissionHistory
{
    private readonly List<SubmissionDocument> submissions = new();
    private int index;

    public SubmissionDocument Current => submissions[index];

    public bool IsEmpty => submissions.Count == 0;

    public void Add(SubmissionDocument submission)
    {
        submissions.Add(submission);
        index = submissions.Count - 1;
    }

    public void MoveNext()
    {
        index = (index + 1) % submissions.Count;
    }

    public void MoveBack()
    {
        var idx = (index - 1) % submissions.Count;

        index = idx >= 0 ? idx : submissions.Count + idx;
    }
}