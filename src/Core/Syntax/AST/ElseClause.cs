using Core.Execution;
using Core.Execution.DataModel.Objects;
using Core.Lexing;
using Core.Utils.Text;

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

        public override SyntaxToken FirstChild => Keyword;

        public override SyntaxToken LastChild => Statement.LastChild;
        
        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}