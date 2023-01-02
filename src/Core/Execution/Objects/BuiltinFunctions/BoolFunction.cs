using System.Collections.Immutable;
using Core.Execution.Interrupts;

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
                context =>
                {
                    var boolean = context.Scope.Lookup(ParameterName).ToBoolean();

                    throw new ReturnInterrupt(boolean);
                })
        {
        }
    }
}