using System.Collections.Immutable;

namespace Core.Execution.DataModel.Objects.Functions.Builtin
{
    public class NumFunction : BuiltinFunction
    {
        private const string ParameterName = "obj";

        public NumFunction()
            : base(
                "num",
                ImmutableArray.Create(ParameterName),
                ImmutableArray<CallArgument>.Empty,
                context =>
                {
                    var str = context.Arguments[ParameterName].ToString();
                    var parsed = double.TryParse(str, out var value);

                    return parsed ? new Number(value) : new Null();
                })
        {
        }
    }
}