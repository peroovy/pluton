using Core.Execution;
using Core.Execution.Objects;
using Core.Lexing;
using Core.Syntax.AST.Expressions;

namespace Core.Syntax.AST
{
    public class WhileStatement : LoopStatement
    {
        public WhileStatement(
            SyntaxToken keyword,
            SyntaxToken openParenthesis,
            Expression condition,
            SyntaxToken closeParenthesis,
            Statement body)
        {
            Keyword = keyword;
            OpenParenthesis = openParenthesis;
            Condition = condition;
            CloseParenthesis = closeParenthesis;
            Body = body;
        }
        
        public SyntaxToken Keyword { get; }
        
        public SyntaxToken OpenParenthesis { get; }

        public override Expression Condition { get; }
        
        public SyntaxToken CloseParenthesis { get; }

        public override Statement Body { get; }

        public override SyntaxToken FirstChild => Keyword;

        public override SyntaxToken LastChild => Body.LastChild;
        
        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}