using System;

namespace Translator.Core.Execution
{
    public class Object
    {
        public Object(object value)
        {
            Value = value;
        }
        
        public object Value { get; }

        public Type Type => Value?.GetType() ?? NullType;

        public override string ToString() => Value?.ToString() ?? "null";

        private static readonly Type NullType = typeof(Nullable);
    }
}