using Interpreter.Core.Execution;
using Interpreter.Core.Execution.Objects;
using Interpreter.Core.Lexing;

namespace Interpreter.Core.Syntax.AST.Expressions
{
    public class AssignmentExpression : Expression
    {
        public AssignmentExpression(SyntaxToken variable, SyntaxToken operatorToken, Expression expression)
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