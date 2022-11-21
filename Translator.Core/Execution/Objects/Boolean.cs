
namespace Translator.Core.Execution.Objects
{
    public class Boolean : Obj
    {
        public Boolean(object value) : base(value)
        {
        }

        public override ObjectTypes Type => ObjectTypes.Boolean;
        
        public override string ToString() => Value.ToString();

        public static Boolean operator &(Boolean left, Boolean right) =>
            new((bool)left.Value && (bool)right.Value);
        
        public static Boolean operator |(Boolean left, Boolean right) =>
            new((bool)left.Value && (bool)right.Value);
    }
}