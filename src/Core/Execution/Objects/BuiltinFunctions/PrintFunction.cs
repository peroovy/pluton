using System;
using System.Collections.Immutable;
using Core.Execution.Objects.DataModel;

namespace Core.Execution.Objects.BuiltinFunctions
{
    public class PrintFunction : BuiltinFunction
    {
        private const string ObjParameter = "obj";
        private const string EndParameter = "\n";
        
        public PrintFunction() 
            : base(
                "print", 
                ImmutableArray.Create(ObjParameter),
                ImmutableArray.Create(new CallArgument(EndParameter, new String("\n"))), 
                context =>
                {
                    var value = context.Scope.Lookup(ObjParameter).ToString();
                    var end = context.Scope.Lookup(EndParameter).ToString();

                    Console.Write(value + end);

                    return new Null();
                })
        {
        }
    }
}