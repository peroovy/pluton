using System.Collections.Immutable;

namespace Interpreter.Core.Execution.Objects.BuiltinFunctions
{
    public class NumFunction : BuiltinFunction
    {
        private const string ParameterName = "obj";

        public NumFunction() 
            : base(
                "num",
                ImmutableArray.Create(ParameterName), 
                (_, scope, stack) =>
                {
                    var str = scope.Lookup(ParameterName).ToString();
                    var parsed = double.TryParse(str, out var value);
                    
                    stack.PushFunctionResult(parsed ? new Number(value) : new Null());
                })
        {
        }
    }
}