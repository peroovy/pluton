using Core.Execution.Objects;
using Core.Utils.Diagnostic;
using Repl.Utils;

namespace Repl;

public interface IPrinter
{
    void PrintError(string message);

    void PrintDiagnostic(DiagnosticBag diagnosticBag);

    void PrintResult(Obj value);
    
    void PrintBlankLine();
    
    void PrintWelcome();

    void FreezeDocumentStartLine();

    void PrintSubmission(SubmissionDocument document);

    void SetCursorToDocumentEnd(SubmissionDocument document);

    void SetCursorAfterDocument(SubmissionDocument document);
}