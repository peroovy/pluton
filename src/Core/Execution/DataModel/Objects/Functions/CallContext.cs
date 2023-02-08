using System.Collections.Immutable;

namespace Core.Execution.DataModel.Objects.Functions
{
    public class CallContext
    {
        public CallContext(Function callable, ImmutableDictionary<string, Obj> arguments, Scope scope)
        {
            Callable = callable;
            Arguments = arguments;
            Scope = scope;

            foreach (var argument in arguments)
                scope.Assign(argument.Key, argument.Value);
        }

        public Function Callable { get; }

        public ImmutableDictionary<string, Obj> Arguments { get; }

        public Scope Scope { get; }
    }
}