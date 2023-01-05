using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Utils.Text;

namespace Core.Utils.Diagnostic
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

        public void AddError(Location location, string message) 
            => bucket.Add(new Log(Level.Error, message, location));

        public void Clear() => bucket.Clear();

        public IDiagnosticBag Copy() => new DiagnosticBag(bucket.ToList());

        public IEnumerator<Log> GetEnumerator() => bucket.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}