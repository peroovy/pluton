namespace Interpreter.Core.Execution.Objects.MagicMethods
{
    public interface IIndexed
    {
        Obj this[int index] { get; set; }
    }
}