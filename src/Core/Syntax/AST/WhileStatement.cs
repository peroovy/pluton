using System.Collections.Generic;
using Core.Execution;
using Core.Execution.Objects;
using Core.Lexing;
using Core.Syntax.AST.Expressions;
using Core.Utils.Text;

namespace Core.Syntax.AST
{
    public class WhileStatement : Statement
    {
        public WhileStatement(SourceText sourceText, SyntaxToken keyword, Expression condition, Statement body)
            : base(sourceText)
        {
            Keyword = keyword;
            Condition = condition;
            Body = body;
        }
        
        public SyntaxToken Keyword { get; }
        
        public Expression Condition { get; }
        
        public Statement Body { get; }

        public override Obj Accept(IExecutor executor) => executor.Execute(this);
        
        public override IEnumerable<Location> GetChildrenLocations()
        {
            yield return Keyword.Location;
            yield return Condition.Location;
            yield return Body.Location;
        }
    }
}