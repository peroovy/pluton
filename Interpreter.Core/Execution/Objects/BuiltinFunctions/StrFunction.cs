using System;
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
                (_, scope, stack) =>
                {
                    var value = scope.Lookup(ParameterName).ToString();
                    stack.PushFunctionResult(new String(value));
                })
        {
        }
    }
}