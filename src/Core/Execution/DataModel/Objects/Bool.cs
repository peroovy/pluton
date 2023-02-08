namespace Core.Execution.DataModel.Objects
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

        public override string ToString()
        {
            return Value.ToString().ToLower();
        }

        public static Bool __not__(Bool operand)
        {
            return new(!operand.Value);
        }

        public static Bool __and__(Bool left, Bool right)
        {
            return new(left.Value && right.Value);
        }

        public static Bool __or__(Bool left, Bool right)
        {
            return new(left.Value || right.Value);
        }

        public static Bool __eq__(Bool left, Bool right)
        {
            return new(left.Value == right.Value);
        }

        public static Bool __neq__(Bool left, Bool right)
        {
            return new(left.Value != right.Value);
        }
    }
}