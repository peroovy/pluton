using System.Collections.Immutable;

namespace Core.Execution.Objects.BuiltinFunctions
{
    public class BoolFunction : BuiltinFunction
    {
        private const string ParameterName = "obj";
        
        public BoolFunction() 
            : base(
                "bool", 
                ImmutableArray.Create(ParameterName), 
                ImmutableArray<(string name, Obj value)>.Empty,
                context => context.Scope.Lookup(ParameterName).ToBoolean())
        {
        }
    }
}