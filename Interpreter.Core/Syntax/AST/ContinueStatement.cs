using Interpreter.Core.Execution;
using Interpreter.Core.Execution.Objects;
using Interpreter.Core.Lexing;

namespace Interpreter.Core.Syntax.AST
{
    public class ContinueStatement : Statement
    {
        public ContinueStatement(SyntaxToken keyword, SyntaxToken semicolon)
        {
            Keyword = keyword;
            Semicolon = semicolon;
        }
        
        public SyntaxToken Keyword { get; }
        
        public SyntaxToken Semicolon { get; }

        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}