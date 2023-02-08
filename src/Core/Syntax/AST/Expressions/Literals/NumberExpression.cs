using Core.Execution;
using Core.Execution.DataModel.Objects;
using Core.Lexing;

namespace Core.Syntax.AST.Expressions.Literals
{
    public class NumberExpression : LiteralExpression
    {
        public NumberExpression(SyntaxToken token) : base(token)
        {
        }

        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}