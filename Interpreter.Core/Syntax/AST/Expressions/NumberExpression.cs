using Interpreter.Core.Execution;
using Interpreter.Core.Execution.Objects;

namespace Interpreter.Core.Syntax.AST.Expressions
{
    public class NumberExpression : Expression
    {
        public NumberExpression(double value)
        {
            Value = value;
        }
        
        public double Value { get; }

        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}