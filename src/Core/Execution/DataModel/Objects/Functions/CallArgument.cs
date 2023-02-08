namespace Core.Execution.DataModel.Objects.Functions
{
    public class CallArgument
    {
        public CallArgument(string name, Obj value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }

        public Obj Value { get; }
    }
}