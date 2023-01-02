using Core.Execution;
using Core.Execution.Objects;
using Core.Lexing;
using Core.Syntax.AST.Expressions;

namespace Core.Syntax.AST
{
    public class ExpressionStatement : Statement
    {

        public ExpressionStatement(Expression expression, SyntaxToken closingToken)
        {
            Expression = expression;
            ClosingToken = closingToken;
        }
        
        public Expression Expression { get; }
        
        public SyntaxToken ClosingToken { get; }

        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}