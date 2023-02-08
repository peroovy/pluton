using System.Collections.Immutable;

namespace Core.Execution.DataModel.Objects.Functions.Builtin
{
    public class StrFunction : BuiltinFunction
    {
        private const string ParameterName = "obj";

        public StrFunction()
            : base(
                "str",
                ImmutableArray.Create(ParameterName),
                ImmutableArray<CallArgument>.Empty,
                context =>
                {
                    var value = context.Scope.Lookup(ParameterName).ToString();

                    return new String(value);
                })
        {
        }
    }
}