using Core.Execution;
using Core.Execution.DataModel.Objects;
using Core.Lexing;

namespace Core.Syntax.AST.Expressions
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

        public override SyntaxToken FirstChild => OpenParenthesis;

        public override SyntaxToken LastChild => CloseParenthesis;
        
        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}