using Core.Execution;
using Core.Execution.Objects;

namespace Core.Syntax.AST.Expressions
{
    public class NullExpression : Expression
    {
        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}