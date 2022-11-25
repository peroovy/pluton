using Interpreter.Core.Execution;
using Interpreter.Core.Execution.Objects;
using Interpreter.Core.Lexing;
using Interpreter.Core.Syntax.AST.Expressions;

namespace Interpreter.Core.Syntax.AST
{
    public class ExpressionStatement : Statement
    {

        public ExpressionStatement(Expression expression, SyntaxToken closingToken)
        {
            Expression = expression;
            ClosingToken = closingToken;
        }
        
        public Expression Expression { get; }
        
        public SyntaxToken ClosingToken { get; }

        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}