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

        public void AddError(TextLocation location, int length, string message) => Report(location, length, message, Level.Error);

        public void Reset() => bucket.Clear();

        private void Report(TextLocation location, int length, string message, Level level)
        {
            var msg = $"{level.ToString().ToUpper()}({location.Line.Number}, {location.Position}): {message}";
            
            bucket.Add(new Log(level, msg, location, length));
        }
    }
}