using Core.Execution;
using Core.Execution.Objects;
using Core.Lexing;

namespace Core.Syntax.AST
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