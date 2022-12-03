using System;
using System.Text;
using Interpreter.Core.Execution.Objects.MagicMethods;

namespace Interpreter.Core.Execution.Objects
{
    public class String : Obj, IIndexReadable
    {
        public String(object value) : base(value)
        {
        }

        public override ObjectTypes Type => ObjectTypes.String;

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

        public override string ToString() => (string)Value;

        public override Boolean ToBoolean() => new(ToString().Length > 0);
        
        public override bool Equals(object obj) => ReferenceEquals(this, obj) || obj is String str && Equals(str);

        public override int GetHashCode() => ToString().GetHashCode();

        private bool Equals(String other) => ToString() == other.ToString();
        
        private bool IsInBound(int index) => index >= 0 && index < ToString().Length;

        private int NormalizeIndex(int index) => index >= 0 ? index : ToString().Length + index;

        public static String operator +(String left, String right) => new(left.ToString() + right);
        
        public static String operator *(String str, Number number)
        {
            var result = new StringBuilder();

            var amount = (int)Math.Round(number.ToDouble());
            for (var i = 0; i < amount; i++)
                result.Append((string)str.Value);

            return new String(result.ToString());
        }

        public static Boolean operator ==(String left, String right) => new(left.Equals(right));

        public static Boolean operator !=(String left, String right) => !(left == right);
    }
}