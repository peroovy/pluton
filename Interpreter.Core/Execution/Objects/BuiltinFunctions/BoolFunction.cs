using System.Collections.Immutable;

namespace Interpreter.Core.Execution.Objects.BuiltinFunctions
{
    public class BoolFunction : Function
    {
        private const string ParameterName = "obj";
        
        public BoolFunction() 
            : base(
                "bool", 
                ImmutableArray.Create(ParameterName), 
                (_, scope, stack) =>
                {
                    var boolean = scope.Lookup(ParameterName).ToBoolean();
                    stack.PushFunctionResult(boolean);
                },
                isBuiltin: true)
        {
        }
    }
}