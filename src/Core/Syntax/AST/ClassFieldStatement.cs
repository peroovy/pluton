using Core.Lexing;
using Core.Syntax.AST.Expressions;

namespace Core.Syntax.AST
{
    public class ClassFieldStatement : VariableAssignmentExpression
    {
        public ClassFieldStatement(
            SyntaxToken identifier, 
            SyntaxToken equalsToken, 
            Expression expression, 
            SyntaxToken semicolon) : base(identifier, equalsToken, expression)
        {
            Semicolon = semicolon;
        }
        
        public override SyntaxToken LastChild => Semicolon;

        public SyntaxToken Semicolon { get; }
    }
}