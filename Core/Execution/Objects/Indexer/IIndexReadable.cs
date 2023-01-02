namespace Core.Execution.Objects.Indexer
{
    public interface IIndexReadable
    {
        Obj this[int index] { get; }
    }
}