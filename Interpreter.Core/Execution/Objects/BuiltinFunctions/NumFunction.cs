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
                ImmutableArray<(string name, Obj value)>.Empty, 
                context =>
                {
                    var str = context.Scope.Lookup(ParameterName).ToString();
                    var parsed = double.TryParse(str, out var value);
                    
                    context.CallStack.PushFunctionResult(parsed ? new Number(value) : new Null());
                })
        {
        }
    }
}