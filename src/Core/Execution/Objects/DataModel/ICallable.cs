using System;
using System.Collections.Immutable;

namespace Core.Execution.Objects.DataModel
{
    public interface ICallable
    {
        ImmutableArray<string> PositionParameters { get; }
        
        ImmutableArray<CallArgument> DefaultParameters { get; }
        
        Func<CallContext, Obj> Invoke { get; }
    }
}