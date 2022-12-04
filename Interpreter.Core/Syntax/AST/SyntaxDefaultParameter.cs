using Interpreter.Core.Lexing;
using Interpreter.Core.Syntax.AST.Expressions;

namespace Interpreter.Core.Syntax.AST
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