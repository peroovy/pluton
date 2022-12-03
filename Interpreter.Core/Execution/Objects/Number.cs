using System;
using System.Globalization;

namespace Interpreter.Core.Execution.Objects
{
    public class Number : Obj
    {
        public Number(double value) : base(value)
        {
        }

        public override ObjectTypes Type => ObjectTypes.Number;

        public bool IsInteger
        {
            get
            {
                var value = ToDouble();
                return Math.Abs(Math.Round(value) - value) < double.Epsilon;
            }
        }

        public override string ToString()
        {
            var value = (double)Value;

            return double.IsNaN(value) ? "NaN" : value.ToString(CultureInfo.InvariantCulture);
        }

        public override Boolean ToBoolean() => new(Convert.ToBoolean(ToDouble()));

        public double ToDouble() => (double)Value;
        
        public override bool Equals(object obj) => ReferenceEquals(this, obj) || obj is Number number && Equals(number);

        public override int GetHashCode() => ToDouble().GetHashCode();

        private bool Equals(Number other) => Math.Abs(ToDouble() - other.ToDouble()) < double.Epsilon;

        public static Number operator +(Number operand) => new(operand.ToDouble());

        public static Number operator -(Number operand) => new(-operand.ToDouble());

        public static Number operator +(Number left, Number right) => new(left.ToDouble() + right.ToDouble());
        
        public static Number operator -(Number left, Number right) => new(left.ToDouble() - right.ToDouble());
        
        public static Number operator *(Number left, Number right) => new(left.ToDouble() * right.ToDouble());
        
        public static String operator *(Number number, String str) => str * number;
        
        public static Array operator *(Number number, Array array) => array * number;

        public static Number operator /(Number left, Number right)
        {
            var rightValue = right.ToDouble();

            return new Number(rightValue == 0 ? double.NaN : left.ToDouble() / rightValue);
        }

        public static Number operator %(Number left, Number right) => new(left.ToDouble() % right.ToDouble());

        public static Boolean operator <(Number left, Number right) => new(left.ToDouble() < right.ToDouble());

        public static Boolean operator >(Number left, Number right) => new(left.ToDouble() > right.ToDouble());
        
        public static Boolean operator <=(Number left, Number right) => new(left.ToDouble() <= right.ToDouble());

        public static Boolean operator >=(Number left, Number right) => new(left.ToDouble() >= right.ToDouble());
        
        public static Boolean operator ==(Number left, Number right) => new(left.Equals(right));

        public static Boolean operator !=(Number left, Number right) => !(left == right);
    }
}