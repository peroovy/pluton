namespace Core.Execution.DataModel.Objects
{
    public class Null : Obj
    {
        private static Null instance;
        private static readonly Class BaseClass = new(nameof(Null));

        private Null() : base(BaseClass)
        {
        }

        public static Null Instance => instance ??= new Null();

        public override string ToString()
        {
            return "null";
        }

        protected override bool IsTrue()
        {
            return false;
        }
    }
}