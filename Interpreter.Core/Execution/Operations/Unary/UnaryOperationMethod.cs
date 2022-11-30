using System.Reflection;
using Interpreter.Core.Execution.Objects;

namespace Interpreter.Core.Execution.Operations.Unary
{
    public class UnaryOperationMethod
    {
        private readonly MethodInfo method;

        public bool IsUnknown => method is null;

        public UnaryOperationMethod(MethodInfo method)
        {
            this.method = method;
        }

        public Obj Invoke(Obj operand)
            => (Obj)method?.Invoke(null, new object[] { operand }) ?? new Null();
    }
}