using System.Linq;
using Core.Utils.Diagnostic;

namespace Core
{
    public class TranslationState<T>
    {
        public TranslationState(T result, DiagnosticBag diagnosticBag)
        {
            Result = result;
            DiagnosticBag = diagnosticBag;
        }
        
        public T Result { get; }
        
        public DiagnosticBag DiagnosticBag { get; }

        public bool HasErrors => DiagnosticBag.Any(log => log.Level == Level.Error);
    }
}