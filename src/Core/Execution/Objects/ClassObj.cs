namespace Core.Execution.Objects
{
    public class ClassObj : Obj
    {
        public ClassObj(string name)
        {
            Name = name;
        }

        public override string AsDebugString => ToString();

        public string Name { get; }

        public override string ToString() => $"class <{Name}>";
    }
}