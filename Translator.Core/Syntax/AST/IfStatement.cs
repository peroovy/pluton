using Translator.Core.Execution;
using Translator.Core.Execution.Objects;
using Translator.Core.Lexing;
using Translator.Core.Syntax.AST.Expressions;

namespace Translator.Core.Syntax.AST
{
    public class IfStatement : Statement
    {
        public IfStatement(SyntaxToken keyword, Expression condition, Statement statement, ElseClause elseClause)
        {
            Keyword = keyword;
            Condition = condition;
            Statement = statement;
            ElseClause = elseClause;
        }
        
        public SyntaxToken Keyword { get; }
        
        public Expression Condition { get; }
        
        public Statement Statement { get; }
        
        public ElseClause ElseClause { get; }

        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}