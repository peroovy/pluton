
namespace Translator.Core.Execution.Objects
{
    public class Boolean : Obj
    {
        public Boolean(object value) : base(value)
        {
        }

        public override ObjectTypes Type => ObjectTypes.Boolean;

        public bool IsTrue => (bool)Value;
        
        public override string ToString() => Value.ToString();
        
        public override Boolean ToBoolean() => new(Value);

        public static Boolean operator !(Boolean operand) => new(!(bool)operand.Value);

        public static Boolean operator &(Boolean left, Boolean right) => new((bool)left.Value && (bool)right.Value);
        
        public static Boolean operator |(Boolean left, Boolean right) => new((bool)left.Value && (bool)right.Value);
    }
}