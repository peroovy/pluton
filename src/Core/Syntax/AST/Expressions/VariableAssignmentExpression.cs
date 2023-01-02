using Core.Execution;
using Core.Execution.Objects;
using Core.Lexing;

namespace Core.Syntax.AST.Expressions
{
    public class VariableAssignmentExpression : Expression
    {
        public VariableAssignmentExpression(SyntaxToken variable, SyntaxToken operatorToken, Expression expression)
        {
            Variable = variable;
            OperatorToken = operatorToken;
            Expression = expression;
        }
        
        public SyntaxToken Variable { get; }
        
        public SyntaxToken OperatorToken { get; }
        
        public Expression Expression { get; }

        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}