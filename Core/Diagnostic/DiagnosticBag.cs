using System.Collections.Generic;
using System.Collections.Immutable;
using Core.Lexing;

namespace Core.Diagnostic
{
    public class DiagnosticBag : IDiagnosticBag
    {
        private readonly List<Log> bucket = new();

        public ImmutableArray<Log> Bucket => bucket.ToImmutableArray();

        public bool IsEmpty => bucket.Count == 0;

        public void AddError(TextLocation location, int length, string message) =>
            bucket.Add(new Log(Level.Error, message, location, length));

        public void Reset() => bucket.Clear();
    }
}