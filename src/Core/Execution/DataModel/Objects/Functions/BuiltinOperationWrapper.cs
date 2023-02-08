using System;
using System.Collections.Immutable;

namespace Core.Execution.DataModel.Objects.Functions
{
    public class BuiltinOperationWrapper : Function
    {
        public BuiltinOperationWrapper(string name, ImmutableArray<string> positionParameters, Func<Obj> evaluate)
            : base(name, positionParameters, ImmutableArray<CallArgument>.Empty, _ => evaluate())
        {
        }
    }
}