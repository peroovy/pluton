using System;
using System.Collections.Immutable;

namespace Interpreter.Core.Execution.Objects.BuiltinFunctions
{
    public class PrintFunction : BuiltinFunction
    {
        private const string ObjParameter = "obj";
        private const string EndParameter = "\n";
        
        public PrintFunction() 
            : base(
                "print", 
                ImmutableArray.Create(ObjParameter),
                ImmutableArray.Create<(string, Obj)>((EndParameter, new String("\n"))), 
                (_, scope, _) =>
                {
                    var value = scope.Lookup(ObjParameter).ToString();
                    var end = scope.Lookup(EndParameter).ToString();

                    Console.Write(value + end);
                })
        {
        }
    }
}