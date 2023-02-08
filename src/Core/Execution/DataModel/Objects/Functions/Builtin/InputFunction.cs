using System;
using System.Collections.Immutable;

namespace Core.Execution.DataModel.Objects.Functions.Builtin
{
    public class InputFunction : BuiltinFunction
    {
        private const string ParameterName = "message";

        public InputFunction()
            : base(
                "input",
                ImmutableArray<string>.Empty,
                ImmutableArray.Create(new CallArgument(ParameterName, new String(string.Empty))),
                context =>
                {
                    var message = context.Scope.Lookup(ParameterName).ToString();

                    Console.Write(message);
                    var value = Console.ReadLine();

                    return new String(value);
                })
        {
        }
    }
}