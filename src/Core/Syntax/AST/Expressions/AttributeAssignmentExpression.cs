using Core.Execution;
using Core.Execution.DataModel.Objects;
using Core.Lexing;

namespace Core.Syntax.AST.Expressions
{
    public class AttributeAssignmentExpression : Expression
    {
        public AttributeAssignmentExpression(
            AttributeAccessExpression accessExpression, SyntaxToken equalsToken, Expression value)
        {
            AccessExpression = accessExpression;
            EqualsToken = equalsToken;
            Value = value;
        }

        public override SyntaxToken FirstChild => AccessExpression.FirstChild;

        public override SyntaxToken LastChild => Value.LastChild;
        
        public AttributeAccessExpression AccessExpression { get; }
        
        public SyntaxToken EqualsToken { get; }
        
        public Expression Value { get; }

        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}