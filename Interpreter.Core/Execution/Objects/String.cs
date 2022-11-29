using System.Text;

namespace Interpreter.Core.Execution.Objects
{
    public class String : Obj
    {
        public String(object value) : base(value)
        {
        }

        public override ObjectTypes Type => ObjectTypes.String;

        public override string ToString() => (string)Value;

        public override Boolean ToBoolean() => new(ToString().Length > 0);

        public static String operator +(String left, String right) => new(left.ToString() + right);
        
        public static String operator *(String str, Number number)
        {
            var result = new StringBuilder();

            var amount = (int)number.ToDouble();
            for (var i = 0; i < amount; i++)
                result.Append((string)str.Value);

            return new String(result.ToString());
        }

        public static Boolean operator ==(String left, String right) => new(left.ToString() == right.ToString());

        public static Boolean operator !=(String left, String right) => !(left == right);
    }
}