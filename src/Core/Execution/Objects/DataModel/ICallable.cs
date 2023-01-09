using System;
using System.Collections.Immutable;

namespace Core.Execution.Objects.DataModel
{
    public interface ICallable
    {
        ImmutableArray<string> PositionParameters { get; }
        
        ImmutableArray<(string Name, Obj Value)> DefaultParameters { get; }
        
        Action<CallContext> Invoke { get; }
    }
}