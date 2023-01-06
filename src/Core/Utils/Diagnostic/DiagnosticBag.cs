using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Utils.Text;

namespace Core.Utils.Diagnostic
{
    public class DiagnosticBag : IEnumerable<Diagnostic>
    {
        private readonly List<Diagnostic> bucket;

        public DiagnosticBag()
        {
            bucket = new List<Diagnostic>();
        }

        private DiagnosticBag(List<Diagnostic> bucket)
        {
            this.bucket = bucket;
        }

        public void AddError(Location location, string message) =>
            bucket.Add(new Diagnostic(Level.Error, message, location));

        public DiagnosticBag Concat(DiagnosticBag other)
        {
            var concatenatedBucket = bucket
                .Concat(other.bucket)
                .ToList();

            return new DiagnosticBag(concatenatedBucket);
        }

        public IEnumerator<Diagnostic> GetEnumerator() => bucket.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}