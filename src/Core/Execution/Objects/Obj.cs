using System.Collections.Generic;

namespace Core.Execution.Objects
{
    public class Obj
    {
        private readonly ClassObj baseClass;
        private readonly Dictionary<string, Obj> attributes = new();

        public Obj(ClassObj baseClass)
        {
            this.baseClass = baseClass;
            TypeName = baseClass.Name;
        }
        
        protected Obj()
        {
            TypeName = GetType().Name;
        }
        
        public virtual string TypeName { get; }

        public virtual string AsDebugString => ToString();

        public override string ToString() => $"object <{TypeName}>";

        public void SetAttribute(string name, Obj value) => attributes[name] = value;

        public bool TryGetAttribute(string name, out Obj value) => attributes.TryGetValue(name, out value);
    }
}