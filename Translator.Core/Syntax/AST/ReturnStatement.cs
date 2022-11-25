using Translator.Core.Execution;
using Translator.Core.Execution.Objects;
using Translator.Core.Lexing;
using Translator.Core.Syntax.AST.Expressions;

namespace Translator.Core.Syntax.AST
{
    public class ReturnStatement : Statement
    {
        public ReturnStatement(SyntaxToken keyword, Expression expression, SyntaxToken semicolon)
        {
            Keyword = keyword;
            Expression = expression;
            Semicolon = semicolon;
        }
        
        public SyntaxToken Keyword { get; }
        
        public Expression Expression { get; }
        
        public SyntaxToken Semicolon { get; }
        
        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}