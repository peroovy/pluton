using System;
using System.Text;
using System.Text.RegularExpressions;
using Core.Execution.Objects.DataModel;

namespace Core.Execution.Objects
{
    public class String : Obj, IIndexReadable, ICollection
    {
        public String(string value)
        {
            Value = value;
        }
        
        public string Value { get; }

        public int Length => Value.Length;

        public Obj this[int index]
        {
            get
            {
                index = NormalizeIndex(index);
                    
                if (!IsInBound(index))
                    throw new IndexOutOfRangeException();

                return new String(ToString()[index].ToString());
            }
        }

        public override string AsDebugString => Regex.Escape($"\"{Value}\"");

        public override string ToString() => Value;

        public override bool Equals(object obj) => ReferenceEquals(this, obj) || obj is String str && Equals(str);

        public override int GetHashCode() => Value.GetHashCode();

        private bool Equals(String other) => Value == other.Value;
        
        private bool IsInBound(int index) => index >= 0 && index < Value.Length;

        private int NormalizeIndex(int index) => index >= 0 ? index : Value.Length + index;

        public static String operator +(String left, String right) => new(left.Value + right.Value);
        
        public static String operator *(String str, Number number)
        {
            var result = new StringBuilder();

            var amount = (int)Math.Round(number.Value);
            for (var i = 0; i < amount; i++)
                result.Append(str.Value);

            return new String(result.ToString());
        }

        public static Boolean operator ==(String left, String right) => new(left.Equals(right));

        public static Boolean operator !=(String left, String right) => new(!left.Equals(right));

        public static Boolean operator <(String left, String right) =>
            new(string.CompareOrdinal(left.Value, right.Value) < 0);

        public static Boolean operator >(String left, String right) =>
            new(string.CompareOrdinal(left.Value, right.Value) > 0);
        
        public static Boolean operator <=(String left, String right) =>
            new(string.CompareOrdinal(left.Value, right.Value) <= 0);

        public static Boolean operator >=(String left, String right) =>
            new(string.CompareOrdinal(left.Value, right.Value) >= 0);
    }
}