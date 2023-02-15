using System;
using System.Collections.Immutable;

namespace Core.Execution.DataModel.Objects.Functions
{
    public class Function : Obj
    {
        private static readonly ClassObj BaseClassObj = new(nameof(Function));

        public Function(
            string name,
            ImmutableArray<string> positionParameters,
            ImmutableArray<CallArgument> defaultParameters,
            Func<CallContext, Obj> invoke) : base(BaseClassObj)
        {
            Name = name;
            PositionParameters = positionParameters;
            DefaultParameters = defaultParameters;
            Invoke = invoke;
        }

        public string Name { get; }

        public ImmutableArray<string> PositionParameters { get; }

        public ImmutableArray<CallArgument> DefaultParameters { get; }

        public Func<CallContext, Obj> Invoke { get; }

        public override string TypeName => "Function";

        public override string ToString()
        {
            return $"function <{Name}>";
        }
    }
}