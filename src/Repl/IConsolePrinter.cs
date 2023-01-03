using Core.Diagnostic;
using Core.Execution.Objects;

namespace Repl;

public interface IConsolePrinter
{
    void PrintError(string message);

    void PrintDiagnostic(IDiagnosticBag diagnostic);

    void PrintInterpretationResult(Obj value);
}