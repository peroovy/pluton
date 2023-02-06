using System.Collections.Generic;

namespace Core.Execution.Objects
{
    // TODO: default __eq__ and __neq__
    public class Obj
    {
        private readonly Dictionary<string, Obj> attributes = new();

        public Obj(ClassObj baseClass)
        {
            BaseClass = baseClass;
            TypeName = baseClass?.Name ?? GetType().Name;
        }

        public ClassObj BaseClass { get; }
        
        public virtual string TypeName { get; }

        public virtual string AsDebugString => ToString();

        public override string ToString() => $"object <{TypeName}>";

        public void SetAttribute(string name, Obj value) => attributes[name] = value;

        public bool TryGetAttribute(string name, out Obj value)
        {
            return attributes.TryGetValue(name, out value) || TryGetAttributeFromBaseClass(name, out value);
        }

        public bool TryGetAttributeFromBaseClass(string name, out Obj value)
        {
            value = null;
            
            return BaseClass?.TryGetAttribute(name, this, out value) ?? false;
        }

        private bool TryGetAttribute(string name, Obj instance, out Obj value)
        {
            if (!attributes.TryGetValue(name, out value))
                return false;

            if (value is Function function)
                value = new Method(instance, function);

            return true;
        }
    }
}