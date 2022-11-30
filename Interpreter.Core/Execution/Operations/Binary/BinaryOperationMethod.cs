using System.Reflection;
using Interpreter.Core.Execution.Objects;

namespace Interpreter.Core.Execution.Operations.Binary
{
    public class BinaryOperationMethod
    {
        private readonly MethodInfo method;

        public bool IsUnknown => method is null;

        public BinaryOperationMethod(MethodInfo method)
        {
            this.method = method;
        }

        public Obj Invoke(Obj left, Obj right)
            => (Obj)method?.Invoke(null, new object[] {left, right}) ?? new Null();
    }
}