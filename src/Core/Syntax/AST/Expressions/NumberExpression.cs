using Core.Execution;
using Core.Execution.Objects;
using Core.Lexing;
using Core.Utils.Text;

namespace Core.Syntax.AST.Expressions
{
    public class NumberExpression : LiteralExpression
    {
        public NumberExpression(SyntaxToken token) : base(token)
        {
        }

        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}