using System;
using System.Collections.Immutable;
using Core.Execution.Objects.DataModel;

namespace Core.Execution.Objects.BuiltinFunctions
{
    public abstract class BuiltinFunction : Function
    {
        protected BuiltinFunction(
            string name, 
            ImmutableArray<string> positionParameters, 
            ImmutableArray<(string name, Obj value)> defaultParameters, 
            Func<CallContext, Obj> invoke) : base(name, positionParameters, defaultParameters, invoke, isBuiltin: true)
        {
        }
    }
}