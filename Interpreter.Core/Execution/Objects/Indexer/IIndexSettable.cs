namespace Interpreter.Core.Execution.Objects.Indexer
{
    public interface IIndexSettable
    {
        Obj this[int index] { set; }
    }
}