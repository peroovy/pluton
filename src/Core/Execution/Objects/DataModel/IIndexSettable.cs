namespace Core.Execution.Objects.DataModel
{
    public interface IIndexSettable
    {
        Obj this[int index] { set; }
    }
}