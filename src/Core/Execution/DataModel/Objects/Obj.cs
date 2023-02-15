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

        protected virtual bool IsTrue => true;
        
        public override string ToString()
        {
            return $"object <{TypeName}>";
        }

        public virtual String ToReprObj(IExecutor executor)
        {
            if (!FindMethod(MagicFunctions.Repr, 1, out var method))
                return ToStringObj(executor);
            
            var obj = executor.InvokeCallableObject(method, ImmutableDictionary<string, Obj>.Empty);
            return new String(obj.ToString());
        }

        public virtual String ToStringObj(IExecutor executor)
        {
            if (!FindMethod(MagicFunctions.Str, 1, out var method))
                return new String(ToString());
            
            var obj = executor.InvokeCallableObject(method, ImmutableDictionary<string, Obj>.Empty);
            return new String(obj.ToString());
        }

        public Bool ToBool(IExecutor executor)
        {
            if (!FindMethod(MagicFunctions.Bool, 1, out var method))
                return new Bool(IsTrue);
            
            var obj = executor.InvokeCallableObject(method, ImmutableDictionary<string, Obj>.Empty);
            return new Bool(obj.IsTrue);
        }

        public void SetAttribute(string name, Obj value)
        {
            attributes[name] = value;
        }

        public bool FindAttribute(string name, out Obj value)
        {
            if (attributes.TryGetValue(name, out value))
                return true;

            return baseClass?.FindAttribute(name, this, out value) ?? false;
        }

        public bool FindMethod(string name, int positionParametersNumber, out MethodWrapper method)
        {
            method = null;

            if (!FindAttribute(name, out var attr) || attr is not MethodWrapper wrapper)
                return false;

            if (wrapper.PositionParameters.Length != positionParametersNumber)
                return false;

            method = wrapper;
            return true;
        }

        private bool FindAttribute(string name, Obj instance, out Obj value)
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