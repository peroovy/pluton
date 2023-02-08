using System.Collections.Immutable;
using Core.Execution.DataModel.Magic;

namespace Core.Execution.DataModel.Objects.Functions.Builtin
{
    public class LenFunction : BuiltinFunction
    {
        private const string ParameterName = "obj";

        public LenFunction()
            : base(
                "len",
                ImmutableArray.Create(ParameterName),
                ImmutableArray<CallArgument>.Empty,
                context =>
                {
                    var obj = context.Arguments[ParameterName];

                    Obj value = obj is ICollection collection
                        ? new Number(collection.Length)
                        : new Null();

                    return value;
                })
        {
        }
    }
}