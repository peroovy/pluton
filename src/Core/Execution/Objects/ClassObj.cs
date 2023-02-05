using System;
using System.Collections.Immutable;
using Core.Execution.Objects.DataModel;

namespace Core.Execution.Objects
{
    public class ClassObj : Obj, ICallable
    {
        public ClassObj(string name)
        {
            Name = name;
            Invoke = _ => new Obj(this);
        }

        public override string AsDebugString => ToString();
        
        public ImmutableArray<string> PositionParameters => ImmutableArray<string>.Empty;
        
        public ImmutableArray<CallArgument> DefaultParameters => ImmutableArray<CallArgument>.Empty;
        
        public Func<CallContext, Obj> Invoke { get; }

        public string Name { get; }

        public override string ToString() => $"class <{Name}>";
    }
}