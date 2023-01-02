using System.Collections.Immutable;
using Core.Lexing;

namespace Core.Diagnostic
{
    public interface IDiagnosticBag
    {
        ImmutableArray<Log> Bucket { get; }
        
        bool IsEmpty { get; }
        
        void AddError(TextLocation location, int length, string message);

        void Reset();
    }
}