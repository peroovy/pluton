using System.Collections.Generic;
using Core.Execution.DataModel.Objects.Functions;

namespace Core.Execution.DataModel.Objects
{
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

        public override string ToString()
        {
            return $"object <{TypeName}>";
        }

        public void SetAttribute(string name, Obj value)
        {
            attributes[name] = value;
        }

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
                value = new MethodWrapper(instance, function);

            return true;
        }

        public static Bool __eq__(Obj left, Obj right)
        {
            return new(ReferenceEquals(left, right));
        }

        public static Bool __neq__(Obj left, Obj right)
        {
            return new(!ReferenceEquals(left, right));
        }
    }
}