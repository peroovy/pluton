using System.Collections.Generic;

namespace Translator.Core.Logging
{
    public interface ILogger
    {
        IEnumerable<Log> Bucket { get; }
        
        bool IsEmpty { get; }
        
        void Error(TextLocation location, int length, string message);

        void Reset();
    }
}