using Core.Execution;
using Core.Execution.Objects;
using Core.Lexing;
using Core.Utils.Text;

namespace Core.Syntax.AST.Expressions
{
    public class NumberExpression : LiteralExpression
    {
        public NumberExpression(SourceText sourceText, SyntaxToken token) : base(sourceText, token)
        {
        }

        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}