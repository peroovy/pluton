using Core.Execution;
using Core.Execution.Objects;
using Core.Lexing;

namespace Core.Syntax.AST.Expressions
{
    public class VariableAssignmentExpression : Expression
    {
        public VariableAssignmentExpression(SyntaxToken identifier, SyntaxToken equalsToken, Expression expression)
        {
            Identifier = identifier;
            EqualsToken = equalsToken;
            Expression = expression;
        }
        
        public SyntaxToken Identifier { get; }
        
        public SyntaxToken EqualsToken { get; }
        
        public Expression Expression { get; }

        public override SyntaxToken FirstChild => Identifier;

        public override SyntaxToken LastChild => Expression.LastChild;
        
        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}