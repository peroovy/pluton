using Interpreter.Core.Execution;
using Interpreter.Core.Execution.Objects;
using Interpreter.Core.Lexing;
using Interpreter.Core.Syntax.AST.Expressions;

namespace Interpreter.Core.Syntax.AST
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