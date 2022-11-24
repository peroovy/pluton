using System.Reflection;
using Translator.Core.Execution.Objects;

namespace Translator.Core.Execution.Operations.Binary
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
            => (Obj)method?.Invoke(null, new object[] {left, right}) ?? new Undefined();
    }
}