using System;
using System.Collections.Immutable;
using Core.Execution.Objects.DataModel;

namespace Core.Execution.Objects
{
    public class BuiltinOperationWrapper : Function
    {
        public BuiltinOperationWrapper(string name, ImmutableArray<string> positionParameters, Func<Obj> evaluate)
            : base(name, positionParameters, ImmutableArray<CallArgument>.Empty,_ => evaluate())
        {
        }
    }
}