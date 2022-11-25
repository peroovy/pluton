using System;
using System.Collections.Immutable;

namespace Translator.Core.Execution.Objects
{
    public class Function : Obj
    {
        public Function(string name, ImmutableArray<string> positionArguments, Func<Scope, Obj> execute, bool isBuiltin) 
            : base(execute)
        {
            Name = name;
            PositionArguments = positionArguments;
            Execute = execute;
            IsBuiltin = isBuiltin;
        }

        public override ObjectTypes Type => ObjectTypes.Function;

        public string Name { get; }
        
        public ImmutableArray<string> PositionArguments { get; }
        
        public Func<Scope, Obj> Execute { get; }
        
        public bool IsBuiltin { get; }

        public override string ToString() => (IsBuiltin ? "built-in " : string.Empty) + $"function {Name}";

        public override Boolean ToBoolean() => new(true);
    }
}