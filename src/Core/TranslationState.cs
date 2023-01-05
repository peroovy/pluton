using System.Linq;
using Core.Utils.Diagnostic;

namespace Core
{
    public class TranslationState<T>
    {
        public TranslationState(T result, DiagnosticBag diagnostic)
        {
            Result = result;
            Diagnostic = diagnostic;
        }
        
        public T Result { get; }
        
        public DiagnosticBag Diagnostic { get; }

        public bool HasErrors => Diagnostic.Any(log => log.Level == Level.Error);
    }
}