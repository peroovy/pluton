using System;
using System.Collections.Immutable;
using Core.Execution.Objects.DataModel;

namespace Core.Execution.Objects
{
    public class Function : Obj, ICallable
    {
        public Function(
            string name, 
            ImmutableArray<string> positionParameters, 
            ImmutableArray<CallArgument> defaultParameters,
            Func<CallContext, Obj> invoke, 
            bool isBuiltin = false)
        {
            Name = name;
            PositionParameters = positionParameters;
            DefaultParameters = defaultParameters;
            Invoke = invoke;
            IsBuiltin = isBuiltin;
        }

        public string Name { get; }
        
        public ImmutableArray<string> PositionParameters { get; }
        
        public ImmutableArray<CallArgument> DefaultParameters { get; }

        public Func<CallContext, Obj> Invoke { get; }
        
        public bool IsBuiltin { get; }

        public override string TypeName => "Function";

        public override string AsDebugString => ToString();

        public override string ToString() => (IsBuiltin ? "built-in " : string.Empty) + $"function <{Name}>";

        public override bool Equals(object obj) => obj is Function function && Equals(function);

        public override int GetHashCode() => Invoke.GetHashCode();

        private bool Equals(Function other) => ReferenceEquals(this, other);

        public static Bool operator ==(Function left, Function right) => new(left.Equals(right));
        
        public static Bool operator !=(Function left, Function right) => new(!left.Equals(right));
    }
}