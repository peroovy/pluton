using System.Collections.Generic;
using Core.Lexing;

namespace Core.Diagnostic
{
    public interface IDiagnosticBag : IEnumerable<Log>
    {
        bool IsEmpty { get; }
        
        void AddError(TextLocation location, int length, string message);
        
        void Clear();
        
        IDiagnosticBag Copy();
    }
}