using Core.Execution;
using Core.Execution.DataModel.Objects;
using Core.Lexing;
using Core.Syntax.AST.Expressions;

namespace Core.Syntax.AST
{
    public class ExpressionStatement : Statement
    {
        public ExpressionStatement(Expression expression, SyntaxToken semicolon)
        {
            Expression = expression;
            Semicolon = semicolon;
        }
        
        public Expression Expression { get; }
        
        public SyntaxToken Semicolon { get; }

        public override SyntaxToken FirstChild => Expression.FirstChild;

        public override SyntaxToken LastChild => Semicolon;
        
        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}