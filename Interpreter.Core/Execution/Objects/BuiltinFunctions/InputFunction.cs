using System;
using System.Collections.Immutable;

namespace Interpreter.Core.Execution.Objects.BuiltinFunctions
{
    public class InputFunction : BuiltinFunction
    {
        private const string ParameterName = "message";

        public InputFunction() 
            : base(
                "input",
                ImmutableArray<string>.Empty,
                ImmutableArray.Create<(string, Obj)>((ParameterName, new String(string.Empty))),
                context =>
                {
                    var message = context.Scope.Lookup(ParameterName).ToString();
                    
                    Console.Write(message);
                    var value = Console.ReadLine();
                    
                    context.CallStack.PushFunctionResult(new String(value));
                })
        {
        }
    }
}