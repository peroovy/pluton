using System.Collections.Generic;
using System.Text;

namespace Translator.Core.Logging
{
    public class Logger : ILogger
    {
        private readonly List<Log> bucket = new List<Log>();

        public IEnumerable<Log> Bucket => bucket.AsReadOnly();

        public bool IsEmpty => bucket.Count == 0;

        public void Error(TextLocation location, int length, string message) => Report(location, length, message, Level.Error);

        public void Reset() => bucket.Clear();

        private void Report(TextLocation location, int length, string message, Level level)
        {
            var msg = $"{level.ToString().ToUpper()}({location.Line.Number}, {location.Position}): {message}";
            
            bucket.Add(new Log(level, msg, location, length));
        }
    }
}