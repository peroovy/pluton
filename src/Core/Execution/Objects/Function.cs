using System;
using System.Collections.Immutable;
using Core.Execution.Objects.DataModel;

namespace Core.Execution.Objects
{
    public class Function : Obj
    {
        public Function(
            string name,
            ImmutableArray<string> positionParameters,
            ImmutableArray<CallArgument> defaultParameters,
            Func<CallContext, Obj> invoke)
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

        public override bool Equals(object obj) => obj is Function function && Equals(function);

        public override int GetHashCode() => Invoke.GetHashCode();

        private bool Equals(Function other) => ReferenceEquals(this, other);

        public static Bool operator ==(Function left, Function right) => new(left.Equals(right));
        
        public static Bool operator !=(Function left, Function right) => new(!left.Equals(right));
    }
}