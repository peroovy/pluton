using Core.Execution.Objects;
using Core.Utils.Diagnostic;

namespace Repl;

public interface IPrinter
{
    void PrintError(string message);

    void PrintDiagnostic(DiagnosticBag diagnosticBag);

    void PrintResult(Obj value);
    
    void PrintBlankLine();
    
    void PrintWelcome();
}