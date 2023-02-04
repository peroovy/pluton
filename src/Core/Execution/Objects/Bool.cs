
namespace Core.Execution.Objects
{
    public class Bool : Obj
    {
        public Bool(bool value)
        {
            Value = value;
        }

        public bool Value { get; }

        public override string AsDebugString => ToString();

        public override string ToString() => Value.ToString().ToLower();
        
        public override bool Equals(object obj) => obj is Bool boolean && Equals(boolean);

        public override int GetHashCode() => Value.GetHashCode();

        private bool Equals(Bool other) => Value == other.Value;

        public static Bool operator !(Bool operand) => new(!operand.Value);

        public static Bool operator &(Bool left, Bool right) => new(left.Value && right.Value);
        
        public static Bool operator |(Bool left, Bool right) => new(left.Value || right.Value);

        public static Bool operator ==(Bool left, Bool right) => new(left.Value == right.Value);
        
        public static Bool operator !=(Bool left, Bool right) => new(left.Value != right.Value);
    }
}