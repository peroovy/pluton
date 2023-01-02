using Core.Diagnostic;

namespace Repl
{
    public interface IDiagnosticPrinter
    {
        void Print(IDiagnosticBag diagnosticBag);
    }
}