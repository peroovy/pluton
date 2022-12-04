using Interpreter.Core.Execution.Objects;

namespace Interpreter.Core.Execution
{
    public class FunctionCallContext
    {
        public FunctionCallContext(Function function, Scope scope, CallStack callStack)
        {
            Function = function;
            Scope = scope;
            CallStack = callStack;
        }
        
        public Function Function { get; }
        
        public Scope Scope { get; }
        
        public CallStack CallStack { get; }
    }
}