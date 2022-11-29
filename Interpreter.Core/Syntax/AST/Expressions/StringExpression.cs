using Interpreter.Core.Execution;
using Interpreter.Core.Execution.Objects;

namespace Interpreter.Core.Syntax.AST.Expressions
{
    public class StringExpression : Expression
    {
        public StringExpression(string value)
        {
            Value = value;
        }
        
        public string Value { get; }

        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}