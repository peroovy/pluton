using Translator.Core.Execution;
using Translator.Core.Execution.Objects;
using Translator.Core.Lexing;
using Translator.Core.Syntax.AST.Expressions;

namespace Translator.Core.Syntax.AST
{
    public class IfStatement : Statement
    {
        public IfStatement(
            SyntaxToken keyword, 
            SyntaxToken openParenthesis, 
            Expression condition, 
            SyntaxToken closeParenthesis, 
            Statement statement, 
            ElseClause elseClause)
        {
            Keyword = keyword;
            OpenParenthesis = openParenthesis;
            Condition = condition;
            CloseParenthesis = closeParenthesis;
            Statement = statement;
            ElseClause = elseClause;
        }
        
        public SyntaxToken Keyword { get; }
        
        public SyntaxToken OpenParenthesis { get; }
        
        public Expression Condition { get; }
        
        public SyntaxToken CloseParenthesis { get; }
        
        public Statement Statement { get; }
        
        public ElseClause ElseClause { get; }

        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}