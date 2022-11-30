using Interpreter.Core.Execution;
using Interpreter.Core.Execution.Objects;

namespace Interpreter.Core.Syntax.AST.Expressions
{
    public class NullExpression : Expression
    {
        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}