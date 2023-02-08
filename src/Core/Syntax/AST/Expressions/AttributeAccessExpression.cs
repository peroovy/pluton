using Core.Execution;
using Core.Execution.DataModel.Objects;
using Core.Lexing;

namespace Core.Syntax.AST.Expressions
{
    public class AttributeAccessExpression : Expression
    {
        public AttributeAccessExpression(Expression objExpression, SyntaxToken dot, SyntaxToken attribute)
        {
            ObjExpression = objExpression;
            Dot = dot;
            Attribute = attribute;
        }

        public override SyntaxToken FirstChild => ObjExpression.FirstChild;

        public override SyntaxToken LastChild => Attribute;
        
        public Expression ObjExpression { get; }
        
        public SyntaxToken Dot { get; }
        
        public SyntaxToken Attribute { get; }

        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}