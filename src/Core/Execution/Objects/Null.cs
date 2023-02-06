namespace Core.Execution.Objects
{
    public class Null : Obj
    {
        private static readonly ClassObj BaseClassObj = new(nameof(Null));

        public Null() : base(BaseClassObj)
        {
        }

        public override string AsDebugString => ToString();

        public override string ToString() => "null";
    }
}