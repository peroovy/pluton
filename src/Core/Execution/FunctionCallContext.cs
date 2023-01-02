using Core.Execution.Objects;

namespace Core.Execution
{
    public class FunctionCallContext
    {
        public FunctionCallContext(Function function, Scope scope)
        {
            Function = function;
            Scope = scope;
        }
        
        public Function Function { get; }
        
        public Scope Scope { get; }
    }
}