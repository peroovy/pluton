using Interpreter.Core.Execution;
using Interpreter.Core.Execution.Objects;
using Interpreter.Core.Lexing;
using Interpreter.Core.Syntax.AST.Expressions;

namespace Interpreter.Core.Syntax.AST
{
    public class IfStatement : Statement
    {
        public IfStatement(SyntaxToken keyword, Expression condition, Statement statement, ElseClause elseClause)
        {
            Keyword = keyword;
            Condition = condition;
            Statement = statement;
            ElseClause = elseClause;
        }
        
        public SyntaxToken Keyword { get; }
        
        public Expression Condition { get; }
        
        public Statement Statement { get; }
        
        public ElseClause ElseClause { get; }

        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}