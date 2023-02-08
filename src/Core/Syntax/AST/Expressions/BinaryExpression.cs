using Core.Execution;
using Core.Execution.DataModel.Objects;
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

        public override SyntaxToken FirstChild => Left.FirstChild;

        public override SyntaxToken LastChild => Right.LastChild;
        
        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}