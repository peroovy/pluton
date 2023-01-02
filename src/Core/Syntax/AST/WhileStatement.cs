using Core.Execution;
using Core.Execution.Objects;
using Core.Lexing;
using Core.Syntax.AST.Expressions;

namespace Core.Syntax.AST
{
    public class WhileStatement : Statement
    {
        public WhileStatement(SyntaxToken keyword, Expression condition, Statement body)
        {
            Keyword = keyword;
            Condition = condition;
            Body = body;
        }
        
        public SyntaxToken Keyword { get; }
        
        public Expression Condition { get; }
        
        public Statement Body { get; }

        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}