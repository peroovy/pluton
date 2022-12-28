using Interpreter.Core.Execution.Objects;

namespace Interpreter.Core.Execution
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