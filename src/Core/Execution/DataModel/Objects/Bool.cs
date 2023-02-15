namespace Core.Execution.DataModel.Objects
{
    public class Bool : Obj
    {
        private static readonly Class BaseClass = new(nameof(Bool));
        private readonly bool value;

        public Bool(bool value) : base(BaseClass)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return value
                .ToString()
                .ToLower();
        }
        
        public static implicit operator bool(Bool obj) => obj.value;

        public static Bool __not__(Bool operand)
        {
            return new Bool(!operand.value);
        }

        public static Bool __and__(Bool left, Bool right)
        {
            return new Bool(left.value && right.value);
        }

        public static Bool __or__(Bool left, Bool right)
        {
            return new Bool(left.value || right.value);
        }

        public static Bool __eq__(Bool left, Bool right)
        {
            return new Bool(left.value == right.value);
        }

        public static Bool __neq__(Bool left, Bool right)
        {
            return new Bool(left.value != right.value);
        }
    }
}