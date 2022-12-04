using System.Collections.Immutable;

namespace Interpreter.Core.Execution.Objects.BuiltinFunctions
{
    public class BoolFunction : BuiltinFunction
    {
        private const string ParameterName = "obj";
        
        public BoolFunction() 
            : base(
                "bool", 
                ImmutableArray.Create(ParameterName), 
                ImmutableArray<(string name, Obj value)>.Empty,
                (_, scope, stack) =>
                {
                    var boolean = scope.Lookup(ParameterName).ToBoolean();
                    stack.PushFunctionResult(boolean);
                })
        {
        }
    }
}