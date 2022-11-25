namespace Interpreter.Core.Execution.Objects
{
    public abstract class Obj
    {
        protected Obj(object value)
        {
            Value = value;
        }
        
        public object Value { get; }

        public abstract ObjectTypes Type { get; }

        public abstract override string ToString();

        public abstract Boolean ToBoolean();
    }
}