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
            SourceText sourceText,
            SyntaxToken keyword, 
            SyntaxToken openParenthesis,
            Expression condition, 
            SyntaxToken closeParenthesis,
            Statement thenStatement, ElseClause elseClause)
            : base(sourceText)
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