namespace Core.Execution.Objects.DataModel
{
    public class CallContext
    {
        public CallContext(ICallable callable, Scope scope)
        {
            Callable = callable;
            Scope = scope;
        }
        
        public ICallable Callable { get; }
        
        public Scope Scope { get; }
    }
}