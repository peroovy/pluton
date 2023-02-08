using Core.Execution.DataModel.Objects;

namespace Core.Execution.DataModel.Magic
{
    public interface IIndexReadable
    {
        Obj this[int index] { get; }
    }
}