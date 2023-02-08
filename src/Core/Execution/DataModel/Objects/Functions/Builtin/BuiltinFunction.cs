using System;
using System.Collections.Immutable;

namespace Core.Execution.DataModel.Objects.Functions.Builtin
{
    public abstract class BuiltinFunction : Function
    {
        protected BuiltinFunction(
            string name,
            ImmutableArray<string> positionParameters,
            ImmutableArray<CallArgument> defaultParameters,
            Func<CallContext, Obj> invoke) : base(name, positionParameters, defaultParameters, invoke)
        {
        }

        public override string ToString()
        {
            return $"built-in function <{Name}>";
        }
    }
}