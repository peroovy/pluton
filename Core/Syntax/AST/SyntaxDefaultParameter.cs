using Core.Lexing;
using Core.Syntax.AST.Expressions;

namespace Core.Syntax.AST
{
    public class SyntaxDefaultParameter
    {
        public SyntaxDefaultParameter(SyntaxToken name, SyntaxToken equalsToken, Expression expression)
        {
            Name = name;
            EqualsToken = equalsToken;
            Expression = expression;
        }
        
        public SyntaxToken Name { get; }
        
        public SyntaxToken EqualsToken { get; }
        
        public Expression Expression { get; }
    }
}