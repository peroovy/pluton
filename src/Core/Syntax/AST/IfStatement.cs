using Core.Execution;
using Core.Execution.Objects;
using Core.Lexing;
using Core.Syntax.AST.Expressions;

namespace Core.Syntax.AST
{
    public class IfStatement : Statement
    {
        public IfStatement(
            SyntaxToken keyword, 
            SyntaxToken openParenthesis,
            Expression condition, 
            SyntaxToken closeParenthesis,
            Statement thenStatement, ElseClause elseClause)
        {
            Keyword = keyword;
            OpenParenthesis = openParenthesis;
            Condition = condition;
            CloseParenthesis = closeParenthesis;
            ThenStatement = thenStatement;
            ElseClause = elseClause;
        }
        
        public SyntaxToken Keyword { get; }
        
        public SyntaxToken OpenParenthesis { get; }

        public Expression Condition { get; }
        
        public SyntaxToken CloseParenthesis { get; }

        public Statement ThenStatement { get; }
        
        public ElseClause ElseClause { get; }

        public override SyntaxToken FirstChild => Keyword;

        public override SyntaxToken LastChild => ElseClause.LastChild;
        
        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}