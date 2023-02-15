using System.Collections.Generic;
using System.Collections.Immutable;
using Core.Execution.DataModel.Objects.Functions;

namespace Core.Execution.DataModel.Objects
{
    public class Obj
    {
        private readonly Dictionary<string, Obj> attributes = new();
        private readonly Class baseClass;

        public Obj(Class baseClass)
        {
            this.baseClass = baseClass;
            TypeName = baseClass?.Name ?? GetType().Name;
        }

        public virtual string TypeName { get; }
        
        public override string ToString()
        {
            return $"object <{TypeName}>";
        }

        public virtual String ToReprObj(IExecutor executor)
        {
            if (!TryGetAttributeFromBaseClass(MagicFunctions.Repr, out var attr)
                || attr is not MethodWrapper method
                || method.PositionParameters.Length != 1)
            {
                return ToStringObj(executor);
            }
            
            var obj = executor.InvokeCallableObject(method, ImmutableDictionary<string, Obj>.Empty);
            return new String(obj.ToString());
        }

        public virtual String ToStringObj(IExecutor executor)
        {
            if (!TryGetAttributeFromBaseClass(MagicFunctions.Str, out var attr)
                || attr is not MethodWrapper method
                || method.PositionParameters.Length != 1)
            {
                return new String(ToString());
            }
            
            var obj = executor.InvokeCallableObject(method, ImmutableDictionary<string, Obj>.Empty);
            return new String(obj.ToString());
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

            return baseClass?.TryGetAttribute(name, this, out value) ?? false;
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
            return new Bool(ReferenceEquals(left, right));
        }

        public static Bool __neq__(Obj left, Obj right)
        {
            return new Bool(!ReferenceEquals(left, right));
        }
    }
}