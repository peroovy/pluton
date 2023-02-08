namespace Core.Execution.DataModel.Objects
{
    public class Null : Obj
    {
        private static Null instance;
        private static readonly ClassObj BaseClassObj = new(nameof(Null));

        private Null() : base(BaseClassObj)
        {
        }

        public static Null Instance => instance ??= new Null();

        public override string AsDebugString => ToString();

        public override string ToString()
        {
            return "null";
        }
    }
}