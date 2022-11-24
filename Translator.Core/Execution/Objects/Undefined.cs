namespace Translator.Core.Execution.Objects
{
    public class Undefined : Obj
    {
        public Undefined() : base(null)
        {
        }

        public override ObjectTypes Type => ObjectTypes.Undefined;

        public override string ToString() => "undefined";

        public override Boolean ToBoolean() => new(false);
    }
}