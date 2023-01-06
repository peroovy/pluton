using System.Collections.Generic;
using Core.Execution;
using Core.Execution.Objects;
using Core.Lexing;
using Core.Syntax.AST.Expressions;
using Core.Utils.Text;

namespace Core.Syntax.AST
{
    public class IfStatement : Statement
    {
        public IfStatement(
            SourceText sourceText, SyntaxToken keyword, Expression condition, Statement thenStatement, ElseClause elseClause)
            : base(sourceText)
        {
            Keyword = keyword;
            Condition = condition;
            ThenStatement = thenStatement;
            ElseClause = elseClause;
        }
        
        public SyntaxToken Keyword { get; }
        
        public Expression Condition { get; }
        
        public Statement ThenStatement { get; }
        
        public ElseClause ElseClause { get; }

        public override Obj Accept(IExecutor executor) => executor.Execute(this);
        
        public override IEnumerable<Location> GetChildrenLocations()
        {
            yield return Keyword.Location;
            yield return Condition.Location;
            yield return ThenStatement.Location;
            yield return ElseClause.Location;
        }
    }
}