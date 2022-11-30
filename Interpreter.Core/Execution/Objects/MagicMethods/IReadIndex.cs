namespace Interpreter.Core.Execution.Objects.MagicMethods
{
    public interface IReadIndex
    {
        Obj this[int index] { get; }
    }
}