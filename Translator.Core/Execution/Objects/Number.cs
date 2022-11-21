namespace Translator.Core.Execution.Objects
{
    public class Number : Obj
    {
        public Number(double value) : base(value)
        {
        }

        public override ObjectTypes Type => ObjectTypes.Number;

        public override string ToString() => Value.ToString();

        public static Number operator +(Number left, Number right) => new((double)left.Value + (double)right.Value);
        
        public static Number operator -(Number left, Number right) => new((double)left.Value - (double)right.Value);
        
        public static Number operator *(Number left, Number right) => new((double)left.Value * (double)right.Value);

        public static Number operator /(Number left, Number right)
        {
            var rightValue = (double)right.Value;

            return new Number(rightValue == 0 ? double.NaN : (double)left.Value / rightValue);
        }
    }
}