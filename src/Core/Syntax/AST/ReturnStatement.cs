using System.Collections.Generic;
using Core.Execution;
using Core.Execution.Objects;
using Core.Lexing;
using Core.Syntax.AST.Expressions;
using Core.Utils.Text;

namespace Core.Syntax.AST
{
    public class ReturnStatement : Statement
    {
        public ReturnStatement(SourceText sourceText, SyntaxToken keyword, Expression expression, SyntaxToken semicolon)
            : base(sourceText)
        {
            Keyword = keyword;
            Expression = expression;
            Semicolon = semicolon;
        }
        
        public SyntaxToken Keyword { get; }
        
        public Expression Expression { get; }
        
        public SyntaxToken Semicolon { get; }
        
        public override Obj Accept(IExecutor executor) => executor.Execute(this);
        
        public override IEnumerable<Location> GetChildrenLocations()
        {
            yield return Keyword.Location;
            
            if (Expression is not null)
                yield return Expression.Location;
            
            yield return Semicolon.Location;
        }
    }
}