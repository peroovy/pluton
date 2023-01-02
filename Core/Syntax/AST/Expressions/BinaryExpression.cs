using Core.Execution;
using Core.Execution.Objects;
using Core.Lexing;

namespace Core.Syntax.AST.Expressions
{
    public class BinaryExpression : Expression
    {
        public BinaryExpression(Expression left, SyntaxToken operatorToken, Expression right)
        {
            Left = left;
            OperatorToken = operatorToken;
            Right = right;
        }
        
        public Expression Left { get; }
        
        public SyntaxToken OperatorToken { get; }
        
        public Expression Right { get; }

        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}