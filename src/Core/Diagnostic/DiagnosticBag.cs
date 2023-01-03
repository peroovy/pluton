using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Lexing;

namespace Core.Diagnostic
{
    public class DiagnosticBag : IDiagnosticBag
    {
        private readonly List<Log> bucket;

        public DiagnosticBag()
        {
            bucket = new List<Log>();
        }

        private DiagnosticBag(List<Log> bucket)
        {
            this.bucket = bucket;
        }

        public bool IsEmpty => bucket.Count == 0;

        public void AddError(TextLocation location, int length, string message) =>
            bucket.Add(new Log(Level.Error, message, location, length));

        public void Clear() => bucket.Clear();

        public IDiagnosticBag Copy() => new DiagnosticBag(bucket.ToList());

        public void Reset() => bucket.Clear();

        public DiagnosticBag Concat(DiagnosticBag other)
        {
            var concatenatedBucket = bucket
                .Concat(other.bucket)
                .ToList();

            return new DiagnosticBag(concatenatedBucket);
        }

        public IEnumerator<Log> GetEnumerator() => bucket.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}