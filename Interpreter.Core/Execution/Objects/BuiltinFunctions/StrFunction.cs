using System.Collections.Immutable;

namespace Interpreter.Core.Execution.Objects.BuiltinFunctions
{
    public class StrFunction : BuiltinFunction
    {
        private const string ParameterName = "obj";

        public StrFunction() 
            : base(
                "str",
                ImmutableArray.Create(ParameterName), 
                ImmutableArray<(string name, Obj value)>.Empty, 
                context =>
                {
                    var value = context.Scope.Lookup(ParameterName).ToString();
                    context.CallStack.PushFunctionResult(new String(value));
                })
        {
        }
    }
}