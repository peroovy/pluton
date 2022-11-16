using Translator.Core.Evaluation;
using Translator.Core.Lexing;

namespace Translator.Core.Syntax.AST
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

        public override T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);
    }
}