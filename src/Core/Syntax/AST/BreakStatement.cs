using Core.Execution;
using Core.Execution.Objects;
using Core.Lexing;

namespace Core.Syntax.AST
{
    public class BreakStatement : Statement
    {
        public BreakStatement(SyntaxToken keyword, SyntaxToken semicolon)
        {
            Keyword = keyword;
            Semicolon = semicolon;
        }
        
        public SyntaxToken Keyword { get; }
        
        public SyntaxToken Semicolon { get; }

        public override SyntaxToken FirstChild => Keyword;

        public override SyntaxToken LastChild => Semicolon;
        
        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}