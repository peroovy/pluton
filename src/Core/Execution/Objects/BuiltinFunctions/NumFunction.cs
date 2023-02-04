using System.Collections.Immutable;
using Core.Execution.Objects.DataModel;

namespace Core.Execution.Objects.BuiltinFunctions
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
                    var str = context.Scope.Lookup(ParameterName).ToString();
                    var parsed = double.TryParse(str, out var value);
                    
                    return parsed ? new Number(value) : new Null();
                })
        {
        }
    }
}