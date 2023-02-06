using System;
using System.Collections.Immutable;
using Core.Execution.Objects.DataModel;

namespace Core.Execution.Objects
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

        public override string AsDebugString => ToString();

        public override string ToString() => $"function <{Name}>";

        public static Bool __eq__(Function left, Function right) => new(Equals(left, right));
        
        public static Bool __neq__(Function left, Function right) => new(!Equals(left, right));
        
        private static bool Equals(Function left, Function right) => ReferenceEquals(left, right);
    }
}