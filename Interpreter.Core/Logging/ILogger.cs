using System.Collections.Generic;
using System.Collections.Immutable;
using Interpreter.Core.Lexing;

namespace Interpreter.Core.Logging
{
    public interface ILogger
    {
        ImmutableArray<Log> Bucket { get; }
        
        bool IsEmpty { get; }
        
        void Error(TextLocation location, int length, string message);

        void Reset();
    }
}