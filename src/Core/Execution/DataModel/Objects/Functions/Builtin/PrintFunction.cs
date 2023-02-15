using System;
using System.Collections.Immutable;

namespace Core.Execution.DataModel.Objects.Functions.Builtin
{
    public class PrintFunction : BuiltinFunction
    {
        private const string ObjParameter = "obj";
        private const string EndParameter = "\n";

        public PrintFunction(Lazy<IExecutor> executor)
            : base(
                "print",
                ImmutableArray.Create(ObjParameter),
                ImmutableArray.Create(new CallArgument(EndParameter, new String("\n"))),
                context =>
                {
                    var value = context.Arguments[ObjParameter].ToStringObj(executor.Value);
                    var end = context.Arguments[EndParameter].ToStringObj(executor.Value);

                    Console.Write(value.Value);
                    Console.Write(end.Value);

                    return Null.Instance;
                })
        {
        }
    }
}