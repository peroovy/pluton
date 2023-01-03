using System.Linq;
using Core.Diagnostic;

namespace Core
{
    public class TranslationState<T>
    {
        public TranslationState(T result, IDiagnosticBag diagnostic)
        {
            Result = result;
            Diagnostic = diagnostic;
        }
        
        public T Result { get; }
        
        public IDiagnosticBag Diagnostic { get; }

        public bool HasErrors => Diagnostic.Any(log => log.Level == Level.Error);
    }
}