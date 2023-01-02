namespace Core.Execution.Objects
{
    public abstract class Obj
    {
        protected Obj()
        {
            TypeName = GetType().Name;
        }
        
        public string TypeName { get; }

        public abstract override string ToString();

        public abstract Boolean ToBoolean();
    }
}