
namespace Core.Execution.Objects
{
    public class Boolean : Obj
    {
        public Boolean(bool value)
        {
            Value = value;
        }

        public bool Value { get; }

        public override string AsDebugString => ToString();

        public override string ToString() => Value.ToString().ToLower();
        
        public override bool Equals(object obj) => obj is Boolean boolean && Equals(boolean);

        public override int GetHashCode() => Value.GetHashCode();

        private bool Equals(Boolean other) => Value == other.Value;

        public static Boolean operator !(Boolean operand) => new(!operand.Value);

        public static Boolean operator &(Boolean left, Boolean right) => new(left.Value && right.Value);
        
        public static Boolean operator |(Boolean left, Boolean right) => new(left.Value || right.Value);

        public static Boolean operator ==(Boolean left, Boolean right) => new(left.Value == right.Value);
        
        public static Boolean operator !=(Boolean left, Boolean right) => new(left.Value != right.Value);
    }
}