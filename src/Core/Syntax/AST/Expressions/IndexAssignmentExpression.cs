using Core.Execution;
using Core.Execution.Objects;
using Core.Lexing;

namespace Core.Syntax.AST.Expressions
{
    public class IndexAssignmentExpression : Expression
    {
        public IndexAssignmentExpression(
            Expression indexedExpression, 
            SyntaxIndex index, 
            SyntaxToken equalsToken, 
            Expression value)
        {
            IndexedExpression = indexedExpression;
            Index = index;
            EqualsToken = equalsToken;
            Value = value;
        }
        
        public Expression IndexedExpression { get; }
        
        public SyntaxIndex Index { get; }
        
        public SyntaxToken EqualsToken { get; }

        public Expression Value { get; }

        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}