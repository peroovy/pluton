using System;
using System.Collections.Immutable;

namespace Core.Execution.DataModel.Objects.Functions.Builtin
{
    public class LenFunction : BuiltinFunction
    {
        private const string ParameterName = "obj";

        public LenFunction(Lazy<IExecutor> executor)
            : base(
                "len",
                ImmutableArray.Create(ParameterName),
                ImmutableArray<CallArgument>.Empty,
                context =>
                {
                    var obj = context.Arguments[ParameterName];

                    return obj.FindMethod(MagicFunctions.Len, 1, out var method)
                        ? executor.Value.InvokeCallableObject(method, ImmutableDictionary<string, Obj>.Empty)
                        : Null.Instance;
                })
        {
        }
    }
}