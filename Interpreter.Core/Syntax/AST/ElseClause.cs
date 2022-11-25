using Interpreter.Core.Execution;
using Interpreter.Core.Execution.Objects;
using Interpreter.Core.Lexing;

namespace Interpreter.Core.Syntax.AST
{
    public class ElseClause : Statement
    {
        public ElseClause(SyntaxToken keyword, Statement statement)
        {
            Keyword = keyword;
            Statement = statement;
        }
        
        public SyntaxToken Keyword { get; }
        
        public Statement Statement { get; }

        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}