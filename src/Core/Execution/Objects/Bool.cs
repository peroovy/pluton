
namespace Core.Execution.Objects
{
    public class Bool : Obj
    {
        private static readonly ClassObj BaseClassObj = new(nameof(Bool));

        public Bool(bool value) : base(BaseClassObj)
        {
            Value = value;
        }

        public bool Value { get; }

        public override string AsDebugString => ToString();

        public override string ToString() => Value.ToString().ToLower();
        
        public static Bool __not__(Bool operand) => new(!operand.Value);

        public static Bool __and__(Bool left, Bool right) => new(left.Value && right.Value);
        
        public static Bool __or__(Bool left, Bool right) => new(left.Value || right.Value);

        public static Bool __eq__(Bool left, Bool right) => new(left.Value == right.Value);
        
        public static Bool __neq__(Bool left, Bool right) => new(left.Value != right.Value);
    }
}