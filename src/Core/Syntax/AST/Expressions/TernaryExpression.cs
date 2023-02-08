using Core.Execution;
using Core.Execution.DataModel.Objects;
using Core.Lexing;

namespace Core.Syntax.AST.Expressions
{
    public class TernaryExpression : Expression
    {
        public TernaryExpression(
            Expression condition,
            SyntaxToken questionMark,
            Expression thenExpression,
            SyntaxToken colon,
            Expression elseExpression)
        {
            Condition = condition;
            QuestionMark = questionMark;
            ThenExpression = thenExpression;
            Colon = colon;
            ElseExpression = elseExpression;
        }

        public override SyntaxToken FirstChild => Condition.FirstChild;

        public override SyntaxToken LastChild => ElseExpression.LastChild;
        
        public Expression Condition { get; }
        
        public SyntaxToken QuestionMark { get; }
        
        public Expression ThenExpression { get; }
        
        public SyntaxToken Colon { get; }
        
        public Expression ElseExpression { get; }

        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}