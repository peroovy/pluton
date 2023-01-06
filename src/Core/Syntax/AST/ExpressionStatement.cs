using System.Collections.Generic;
using Core.Execution;
using Core.Execution.Objects;
using Core.Lexing;
using Core.Syntax.AST.Expressions;
using Core.Utils.Text;

namespace Core.Syntax.AST
{
    public class ExpressionStatement : Statement
    {
        public ExpressionStatement(SourceText sourceText, Expression expression, SyntaxToken semicolon) : base(sourceText)
        {
            Expression = expression;
            Semicolon = semicolon;
        }
        
        public Expression Expression { get; }
        
        public SyntaxToken Semicolon { get; }

        public override Obj Accept(IExecutor executor) => executor.Execute(this);
        
        public override IEnumerable<Location> GetChildrenLocations()
        {
            yield return Expression.Location;
            yield return Semicolon.Location;
        }
    }
}