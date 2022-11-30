namespace Interpreter.Core.Execution.Objects
{
    public class Null : Obj
    {
        public Null() : base(null)
        {
        }

        public override ObjectTypes Type => ObjectTypes.Null;

        public override string ToString() => "null";

        public override Boolean ToBoolean() => new(false);
    }
}