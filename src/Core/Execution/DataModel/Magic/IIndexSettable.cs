using Core.Execution.DataModel.Objects;

namespace Core.Execution.DataModel.Magic
{
    public interface IIndexSettable
    {
        Obj this[int index] { set; }
    }
}