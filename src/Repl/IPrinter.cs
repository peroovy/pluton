using Core.Diagnostic;
using Core.Execution.Objects;

namespace Repl;

public interface IPrinter
{
    void PrintError(string message);

    void PrintDiagnostic(IDiagnosticBag diagnostic);

    void PrintResult(Obj value);
}