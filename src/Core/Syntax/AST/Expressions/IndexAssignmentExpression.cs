using Core.Execution;
using Core.Execution.Objects;
using Core.Lexing;

namespace Core.Syntax.AST.Expressions
{
    public class IndexAssignmentExpression : Expression
    {
        public IndexAssignmentExpression(
            Expression expression, 
            SyntaxIndex index, 
            SyntaxToken operatorToken, 
            Expression value)
        {
            Expression = expression;
            Index = index;
            OperatorToken = operatorToken;
            Value = value;
        }
        
        public Expression Expression { get; }
        
        public SyntaxIndex Index { get; }
        
        public SyntaxToken OperatorToken { get; }

        public Expression Value { get; }

        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}