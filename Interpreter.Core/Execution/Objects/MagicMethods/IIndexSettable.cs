namespace Interpreter.Core.Execution.Objects.MagicMethods
{
    public interface IIndexSettable
    {
        Obj this[int index] { set; }
    }
}