﻿using System;
using System.Globalization;

namespace Core.Execution.Objects
{
    public class Number : Obj
    {
        private static readonly ClassObj BaseClassObj = new(nameof(Number));

        public Number(double value) : base(BaseClassObj)
        {
            Value = value;
        }
        
        public double Value { get; }

        public int AsInteger => (int)Value;

        public bool IsInteger => Math.Abs(Math.Round(Value) - Value) < double.Epsilon;

        public override string AsDebugString => ToString();

        public override string ToString() => double.IsNaN(Value) ? "NaN" : Value.ToString(CultureInfo.InvariantCulture);

        public static Number __pos__(Number operand) => new(operand.Value);

        public static Number __neg__(Number operand) => new(-operand.Value);

        public static Number __add__(Number left, Number right) => new(left.Value + right.Value);
        
        public static Number __sub__(Number left, Number right) => new(left.Value - right.Value);
        
        public static Number __mult__(Number left, Number right) => new(left.Value * right.Value);
        
        public static String __mult__(Number number, String str) => String.__mult__(str, number);
        
        public static Array __mult__(Number number, Array array) => Array.__mult__(array, number);

        public static Number __div__(Number left, Number right)
        {
            var rightValue = right.Value;

            return new Number(rightValue == 0 ? double.NaN : left.Value / rightValue);
        }

        public static Number __mod__(Number left, Number right) => new(left.Value % right.Value);

        public static Bool __lt__(Number left, Number right) => new(left.Value < right.Value);

        public static Bool __lte__(Number left, Number right) => new(left.Value <= right.Value);
        
        public static Bool __gt__(Number left, Number right) => new(left.Value > right.Value);

        public static Bool __gte__(Number left, Number right) => new(left.Value >= right.Value);
        
        public static Bool __eq__(Number left, Number right) => new(Equals(left, right));

        public static Bool __neq__(Number left, Number right) => new(!Equals(left, right));
        
        private static bool Equals(Number left, Number right) => Math.Abs(left.Value - right.Value) < double.Epsilon;
    }
}