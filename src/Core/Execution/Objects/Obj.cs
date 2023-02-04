namespace Core.Execution.Objects
{
    public abstract class Obj
    {
        protected Obj()
        {
            TypeName = GetType().Name;
        }
        
        public virtual string TypeName { get; }

        public abstract string AsDebugString { get; }

        public abstract override string ToString();
    }
}