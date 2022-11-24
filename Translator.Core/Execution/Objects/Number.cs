﻿using System;

namespace Translator.Core.Execution.Objects
{
    public class Number : Obj
    {
        public Number(double value) : base(value)
        {
        }

        public override ObjectTypes Type => ObjectTypes.Number;

        public override string ToString() => Value.ToString();

        public override Boolean ToBoolean() => new(Convert.ToBoolean((double)Value));

        public static Number operator +(Number operand) => new((double)operand.Value);

        public static Number operator -(Number operand) => new(-(double)operand.Value);

        public static Number operator +(Number left, Number right) => new((double)left.Value + (double)right.Value);
        
        public static Number operator -(Number left, Number right) => new((double)left.Value - (double)right.Value);
        
        public static Number operator *(Number left, Number right) => new((double)left.Value * (double)right.Value);

        public static Number operator /(Number left, Number right)
        {
            var rightValue = (double)right.Value;

            return new Number(rightValue == 0 ? double.NaN : (double)left.Value / rightValue);
        }

        public static Boolean operator <(Number left, Number right) => new((double)left.Value < (double)right.Value);

        public static Boolean operator >(Number left, Number right) => new((double)left.Value > (double)right.Value);
    }
}