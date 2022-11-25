using System;
using System.Collections.Immutable;

namespace Translator.Core.Execution.Objects
{
    public class Function : Obj
    {
        public Function(string name, ImmutableArray<string> positionArguments, Action<Function, Scope, Stack> call, bool isBuiltin) 
            : base(call)
        {
            Name = name;
            PositionArguments = positionArguments;
            Call = call;
            IsBuiltin = isBuiltin;
        }

        public override ObjectTypes Type => ObjectTypes.Function;

        public string Name { get; }
        
        public ImmutableArray<string> PositionArguments { get; }
        
        public Action<Function, Scope, Stack> Call { get; }
        
        public bool IsBuiltin { get; }

        public override string ToString() => (IsBuiltin ? "built-in " : string.Empty) + $"function {Name}";

        public override Boolean ToBoolean() => new(true);
    }
}