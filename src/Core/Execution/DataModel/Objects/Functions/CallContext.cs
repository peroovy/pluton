using System.Collections.Immutable;

namespace Core.Execution.DataModel.Objects.Functions
{
    public class CallContext
    {
        public CallContext(Function callable, ImmutableDictionary<string, Obj> arguments, Scope globalScope)
        {
            Callable = callable;
            Arguments = arguments;
            Scope = new Scope(globalScope);

            foreach (var argument in arguments)
                Scope.Assign(argument.Key, argument.Value);
        }

        public Function Callable { get; }

        public ImmutableDictionary<string, Obj> Arguments { get; }

        public Scope Scope { get; }
    }
}