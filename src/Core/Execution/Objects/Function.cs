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
            ImmutableArray<(string name, Obj value)> defaultParameters,
            Action<CallContext> invoke, 
            bool isBuiltin)
        {
            Name = name;
            PositionParameters = positionParameters;
            DefaultParameters = defaultParameters;
            Invoke = invoke;
            IsBuiltin = isBuiltin;
        }

        public string Name { get; }
        
        public ImmutableArray<string> PositionParameters { get; }
        
        public ImmutableArray<(string Name, Obj Value)> DefaultParameters { get; }

        public Action<CallContext> Invoke { get; }
        
        public bool IsBuiltin { get; }

        public override string TypeName => "Function";

        public override string AsDebugString => ToString();

        public override string ToString() => (IsBuiltin ? "built-in " : string.Empty) + $"function <{Name}>";

        public override Boolean ToBoolean() => new(true);

        public override bool Equals(object obj) => obj is Function function && Equals(function);

        public override int GetHashCode() => Invoke.GetHashCode();

        private bool Equals(Function other) => ReferenceEquals(this, other);

        public static Boolean operator ==(Function left, Function right) => new(left.Equals(right));
        
        public static Boolean operator !=(Function left, Function right) => new(!left.Equals(right));
    }
}