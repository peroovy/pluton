using System.Collections.Generic;
using Core.Execution;
using Core.Execution.Objects;
using Core.Lexing;
using Core.Utils.Text;

namespace Core.Syntax.AST
{
    public class ElseClause : Statement
    {
        public ElseClause(SourceText sourceText, SyntaxToken keyword, Statement statement) : base(sourceText)
        {
            Keyword = keyword;
            Statement = statement;
        }
        
        public SyntaxToken Keyword { get; }
        
        public Statement Statement { get; }

        public override Obj Accept(IExecutor executor) => executor.Execute(this);
        
        public override IEnumerable<Location> GetChildrenLocations()
        {
            yield return Keyword.Location;
            yield return Statement.Location;
        }
    }
}