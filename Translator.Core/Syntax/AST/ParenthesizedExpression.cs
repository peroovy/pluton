using Translator.Core.Lexing;

namespace Translator.Core.Syntax.AST
{
    public class ParenthesizedExpression : Expression
    {
        public ParenthesizedExpression(SyntaxToken openParenthesis, Expression innerExpression, SyntaxToken closeParenthesis)
        {
            OpenParenthesis = openParenthesis;
            InnerExpression = innerExpression;
            CloseParenthesis = closeParenthesis;
        }
        
        public SyntaxToken OpenParenthesis { get; }
        
        public Expression InnerExpression { get; }
        
        public SyntaxToken CloseParenthesis { get; }
        
        public override T Accept<T>(IVisitor<T> visitor) => visitor.Visit(this);
    }
}