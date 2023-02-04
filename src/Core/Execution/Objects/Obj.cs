using System.Collections.Generic;

namespace Core.Execution.Objects
{
    public abstract class Obj
    {
        private Dictionary<string, Obj> attributes = new();
        
        protected Obj()
        {
            TypeName = GetType().Name;
        }
        
        public virtual string TypeName { get; }

        public abstract string AsDebugString { get; }

        public abstract override string ToString();

        public void SetAttribute(string name, Obj value) => attributes[name] = value;

        public bool TryGetAttribute(string name, out Obj value) => attributes.TryGetValue(name, out value);
    }
}