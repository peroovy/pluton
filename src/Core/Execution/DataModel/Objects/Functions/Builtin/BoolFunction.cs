using System;
using System.Collections.Immutable;

namespace Core.Execution.DataModel.Objects.Functions.Builtin
{
    public class BoolFunction : BuiltinFunction
    {
        private const string ParameterName = "obj";

        public BoolFunction(Lazy<IExecutor> executor)
            : base(
                "bool",
                ImmutableArray.Create(ParameterName),
                ImmutableArray<CallArgument>.Empty,
                context => context.Arguments[ParameterName].ToBool(executor.Value))
        {
        }
    }
}