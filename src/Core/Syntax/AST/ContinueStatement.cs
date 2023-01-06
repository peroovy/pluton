using System.Collections.Generic;
using Core.Execution;
using Core.Execution.Objects;
using Core.Lexing;
using Core.Utils.Text;

namespace Core.Syntax.AST
{
    public class ContinueStatement : Statement
    {
        public ContinueStatement(SourceText sourceText, SyntaxToken keyword, SyntaxToken semicolon) : base(sourceText)
        {
            Keyword = keyword;
            Semicolon = semicolon;
        }
        
        public SyntaxToken Keyword { get; }
        
        public SyntaxToken Semicolon { get; }

        public override Obj Accept(IExecutor executor) => executor.Execute(this);
        
        public override IEnumerable<Location> GetChildrenLocations()
        {
            yield return Keyword.Location;
            yield return Semicolon.Location;
        }
    }
}