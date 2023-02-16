using System.Collections.Immutable;
using Core.Execution.DataModel.Objects.Functions;

namespace Core.Execution.DataModel.Objects
{
    public class Class : Obj
    {
        private static readonly Function DefaultInitializer = new(
            MagicFunctions.Init,
            ImmutableArray.Create("self"),
            ImmutableArray<CallArgument>.Empty,
            _ => Null.Instance);

        public Class(string name, Class subclass = null) : base(null)
        {
            Name = name;
            Subclass = subclass;
            SetAttribute(MagicFunctions.Init, DefaultInitializer);
        }

        public string Name { get; }
        
        public Class Subclass { get; }

        public override string ToString()
        {
            return $"class <{Name}>";
        }
    }
}