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

        public abstract Boolean ToBoolean();

        public static Boolean operator &(Obj left, Obj right) => left.ToBoolean() & right.ToBoolean();
        
        public static Boolean operator |(Obj left, Obj right) => left.ToBoolean() | right.ToBoolean();
    }
}