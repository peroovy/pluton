using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Core.Utils.Diagnostic
{
    public class DiagnosticBag : IEnumerable<Log>
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

        public void AddError(Location location, string message) => bucket.Add(new Log(Level.Error, message, location));

        public void Clear() => bucket.Clear();

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