using Interpreter.Core.Execution;
using Interpreter.Core.Execution.Objects;
using Interpreter.Core.Lexing;
using Interpreter.Core.Syntax.AST.Expressions;

namespace Interpreter.Core.Syntax.AST
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