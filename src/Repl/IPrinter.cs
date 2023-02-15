using Core;
using Core.Execution.DataModel.Objects;
using Core.Utils.Diagnostic;

namespace Repl;

public interface IPrinter
{
    void PrintError(string message);

    void PrintDiagnostic(DiagnosticBag diagnosticBag);

    void PrintResult(Obj value, Interpreter interpreter);
    
    void PrintBlankLine();
    
    void PrintWelcome();

    void PrintHelp();
}