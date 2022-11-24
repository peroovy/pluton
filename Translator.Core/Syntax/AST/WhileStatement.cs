using Translator.Core.Execution;
using Translator.Core.Execution.Objects;
using Translator.Core.Lexing;
using Translator.Core.Syntax.AST.Expressions;

namespace Translator.Core.Syntax.AST
{
    public class WhileStatement : Statement
    {
        public WhileStatement(SyntaxToken keyword, Condition condition, Statement body)
        {
            Keyword = keyword;
            Condition = condition;
            Body = body;
        }
        
        public SyntaxToken Keyword { get; }
        
        public Condition Condition { get; }
        
        public Statement Body { get; }

        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}