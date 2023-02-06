namespace Core.Execution.Objects.DataModel
{
    public class CallContext
    {
        public CallContext(Function callable, Scope scope)
        {
            Callable = callable;
            Scope = scope;
        }
        
        public Function Callable { get; }
        
        public Scope Scope { get; }
    }
}