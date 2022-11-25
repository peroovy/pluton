using System;
using System.Collections.Immutable;

namespace Translator.Core.Execution.Objects.BuiltinFunctions
{
    public class PrintFunction : Function
    {
        private const string ParameterName = "obj";
        
        public PrintFunction() 
            : base(
                "print", 
                ImmutableArray.Create(ParameterName), 
                (_, scope) => Console.WriteLine(scope.Lookup(ParameterName)),
                isBuiltin: true)
        {
        }
    }
}