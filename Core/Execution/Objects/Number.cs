using System;
using System.Globalization;

namespace Core.Execution.Objects
{
    public class Number : Obj
    {
        public Number(double value)
        {
            Value = value;
        }
        
        public double Value { get; }

        public bool IsInteger => Math.Abs(Math.Round(Value) - Value) < double.Epsilon;

        public override string ToRepresentation() => ToString();

        public override string ToString() => double.IsNaN(Value) ? "NaN" : Value.ToString(CultureInfo.InvariantCulture);

        public override Boolean ToBoolean() => new(Convert.ToBoolean(Value));

        public override bool Equals(object obj) => ReferenceEquals(this, obj) || obj is Number number && Equals(number);

        public override int GetHashCode() => Value.GetHashCode();

        private bool Equals(Number other) => Math.Abs(Value - other.Value) < double.Epsilon;

        public static Number operator +(Number operand) => new(operand.Value);

        public static Number operator -(Number operand) => new(-operand.Value);

        public static Number operator +(Number left, Number right) => new(left.Value + right.Value);
        
        public static Number operator -(Number left, Number right) => new(left.Value - right.Value);
        
        public static Number operator *(Number left, Number right) => new(left.Value * right.Value);
        
        public static String operator *(Number number, String str) => str * number;
        
        public static Array operator *(Number number, Array array) => array * number;

        public static Number operator /(Number left, Number right)
        {
            var rightValue = right.Value;

            return new Number(rightValue == 0 ? double.NaN : left.Value / rightValue);
        }

        public static Number operator %(Number left, Number right) => new(left.Value % right.Value);

        public static Boolean operator <(Number left, Number right) => new(left.Value < right.Value);

        public static Boolean operator >(Number left, Number right) => new(left.Value > right.Value);
        
        public static Boolean operator <=(Number left, Number right) => new(left.Value <= right.Value);

        public static Boolean operator >=(Number left, Number right) => new(left.Value >= right.Value);
        
        public static Boolean operator ==(Number left, Number right) => new(left.Equals(right));

        public static Boolean operator !=(Number left, Number right) => new(!left.Equals(right));
    }
}