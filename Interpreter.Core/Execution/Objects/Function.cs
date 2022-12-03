using System;
using System.Collections.Immutable;

namespace Interpreter.Core.Execution.Objects
{
    public class Function : Obj
    {
        public Function(
            string name, 
            ImmutableArray<string> positionParameters, 
            Action<Function, Scope, Stack> call, 
            bool isBuiltin)
        {
            Name = name;
            PositionParameters = positionParameters;
            Call = call;
            IsBuiltin = isBuiltin;
        }

        public string Name { get; }
        
        public ImmutableArray<string> PositionParameters { get; }
        
        public Action<Function, Scope, Stack> Call { get; }
        
        public bool IsBuiltin { get; }

        public override string ToString() => (IsBuiltin ? "built-in " : string.Empty) + $"function <{Name}>";

        public override Boolean ToBoolean() => new(true);

        public override bool Equals(object obj) => obj is Function function && Equals(function);

        public override int GetHashCode() => Call.GetHashCode();

        private bool Equals(Function other) => ReferenceEquals(this, other);

        public static Boolean operator ==(Function left, Function right) => new(left.Equals(right));
        
        public static Boolean operator !=(Function left, Function right) => new(!left.Equals(right));
    }
}