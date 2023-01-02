using System.Reflection;
using Core.Execution.Objects;

namespace Core.Execution.Operations.Binary
{
    public class BinaryOperationMethod
    {
        private readonly MethodInfo method;

        public bool IsUnknown => method is null;

        public BinaryOperationMethod(MethodInfo method)
        {
            this.method = method;
        }

        public Obj Invoke(Obj left, Obj right) => (Obj)method?.Invoke(null, new object[] { left, right });
    }
}