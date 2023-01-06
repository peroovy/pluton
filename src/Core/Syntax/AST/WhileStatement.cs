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
        public WhileStatement(
            SourceText sourceText,
            SyntaxToken keyword,
            SyntaxToken openParenthesis,
            Expression condition,
            SyntaxToken closeParenthesis,
            Statement body)
            : base(sourceText)
        {
            Keyword = keyword;
            OpenParenthesis = openParenthesis;
            Condition = condition;
            CloseParenthesis = closeParenthesis;
            Body = body;
        }
        
        public SyntaxToken Keyword { get; }
        
        public SyntaxToken OpenParenthesis { get; }

        public Expression Condition { get; }
        
        public SyntaxToken CloseParenthesis { get; }

        public Statement Body { get; }

        public override Obj Accept(IExecutor executor) => executor.Execute(this);
        
        public override IEnumerable<Location> GetChildrenLocations()
        {
            yield return Keyword.Location;
            yield return OpenParenthesis.Location;
            yield return Condition.Location;
            yield return CloseParenthesis.Location;
            yield return Body.Location;
        }
    }
}