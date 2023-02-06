using System.Collections.Immutable;
using Core.Execution.Objects.DataModel;

namespace Core.Execution.Objects
{
    public class ClassObj : Obj
    {
        private static readonly Function DefaultInitializer = new(
            MagicFunctions.Initializer,
            ImmutableArray.Create("self"),
            ImmutableArray<CallArgument>.Empty,
            _ => new Null());
        
        public ClassObj(string name)
        {
            Name = name;
            SetAttribute(MagicFunctions.Initializer, DefaultInitializer);
        }

        public override string AsDebugString => ToString();

        public string Name { get; }

        public Method MagicMethodNew
        {
            get
            {
                var initializer = TryGetAttribute(MagicFunctions.Initializer, out var attr) & attr is Function
                    ? (Function)attr
                    : DefaultInitializer;
            
                var magicMethodNew = new Function(
                    MagicFunctions.New, initializer.PositionParameters, initializer.DefaultParameters, 
                    context =>
                    {
                        var instance = new Obj(this);

                        new Method(instance, initializer).Invoke(context);
        
                        return instance;
                    });

                return new Method(this, magicMethodNew);
            }
        }

        public override string ToString() => $"class <{Name}>";
    }
}