using System.Collections.Generic;
using Core.Utils.Text;

namespace Core.Utils.Diagnostic
{
    public interface IDiagnosticBag : IEnumerable<Log>
    {
        bool IsEmpty { get; }
        
        void AddError(Location location, string message);
        
        void Clear();
        
        IDiagnosticBag Copy();
    }
}