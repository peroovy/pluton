namespace Interpreter.Core.Execution.Objects
{
    public class String : Obj
    {
        public String(object value) : base(value)
        {
        }

        public override ObjectTypes Type => ObjectTypes.String;

        public override string ToString() => (string)Value;

        public override Boolean ToBoolean() => new(((string)Value).Length > 0);
    }
}