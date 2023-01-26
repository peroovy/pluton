using Core.Execution;
using Core.Execution.Objects;
using Core.Lexing;

namespace Core.Syntax.AST.Expressions.Literals
{
    public class VariableExpression : LiteralExpression
    {
        public VariableExpression(SyntaxToken token) : base(token)
        {
        }
        
        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}