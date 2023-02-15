using System;
using System.Collections.Immutable;

namespace Core.Execution.DataModel.Objects.Functions.Builtin
{
    public class ReprFunction : BuiltinFunction
    {
        private const string ParameterName = "obj";

        public ReprFunction(Lazy<IExecutor> executor)
            : base(
                "repr",
                ImmutableArray.Create(ParameterName),
                ImmutableArray<CallArgument>.Empty,
                context => context.Arguments[ParameterName].ToReprObj(executor.Value))
        {
        }
    }
}