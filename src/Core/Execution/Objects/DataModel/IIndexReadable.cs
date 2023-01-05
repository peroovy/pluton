namespace Core.Execution.Objects.DataModel
{
    public interface IIndexReadable
    {
        Obj this[int index] { get; }
    }
}