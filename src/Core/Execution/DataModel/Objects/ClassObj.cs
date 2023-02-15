using System.Collections.Immutable;
using Core.Execution.DataModel.Objects.Functions;

namespace Core.Execution.DataModel.Objects
{
    public class ClassObj : Obj
    {
        private static readonly Function DefaultInitializer = new(
            MagicFunctions.Init,
            ImmutableArray.Create("self"),
            ImmutableArray<CallArgument>.Empty,
            _ => Null.Instance);

        public ClassObj(string name) : base(null)
        {
            Name = name;
            SetAttribute(MagicFunctions.Init, DefaultInitializer);
        }

        public string Name { get; }

        public override string ToString()
        {
            return $"class <{Name}>";
        }
    }
}