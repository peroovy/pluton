using System.Collections.Generic;
using Core.Execution;
using Core.Execution.Objects;
using Core.Lexing;
using Core.Utils.Text;

namespace Core.Syntax.AST.Expressions
{
    public class ParenthesizedExpression : Expression
    {
        public ParenthesizedExpression(
            SourceText sourceText, SyntaxToken openParenthesis, Expression innerExpression, SyntaxToken closeParenthesis)
            : base(sourceText)
        {
            OpenParenthesis = openParenthesis;
            InnerExpression = innerExpression;
            CloseParenthesis = closeParenthesis;
        }
        
        public SyntaxToken OpenParenthesis { get; }
        
        public Expression InnerExpression { get; }
        
        public SyntaxToken CloseParenthesis { get; }
        
        public override Obj Accept(IExecutor executor) => executor.Execute(this);
        
        public override IEnumerable<Location> GetChildrenLocations()
        {
            yield return OpenParenthesis.Location;
            yield return InnerExpression.Location;
            yield return CloseParenthesis.Location;
        }
    }
}