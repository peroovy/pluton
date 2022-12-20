using System.Collections.Immutable;
using Interpreter.Core.Execution.Interrupts;
using Interpreter.Core.Execution.Objects.DataModel;

namespace Interpreter.Core.Execution.Objects.BuiltinFunctions
{
    public class LenFunction : BuiltinFunction
    {
        private const string ParameterName = "obj";

        public LenFunction() 
            : base(
                "len", 
                ImmutableArray.Create(ParameterName), 
                ImmutableArray<(string name, Obj value)>.Empty, 
                context =>
                {
                    var obj = context.Scope.Lookup(ParameterName);

                    Obj value = obj is ICollection collection
                        ? new Number(collection.Length)
                        : new Null();
                    
                    throw new ReturnInterrupt(value);
                })
        {
        }
    }
}