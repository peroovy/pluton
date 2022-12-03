namespace Interpreter.Core.Execution.Objects.MagicMethods
{
    public interface IIndexReadable
    {
        Obj this[int index] { get; }
    }
}