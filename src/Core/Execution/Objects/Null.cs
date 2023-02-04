namespace Core.Execution.Objects
{
    public class Null : Obj
    {
        public override string AsDebugString => ToString();

        public override string ToString() => "null";
    }
}