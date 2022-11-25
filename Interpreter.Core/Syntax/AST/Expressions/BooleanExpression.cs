using Interpreter.Core.Execution;
using Interpreter.Core.Execution.Objects;

namespace Interpreter.Core.Syntax.AST.Expressions
{
    public class BooleanExpression : Expression
    {
        public BooleanExpression(bool value)
        {
            Value = value;
        }
        
        public bool Value { get; }
        
        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}