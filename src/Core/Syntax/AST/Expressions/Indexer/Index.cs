using Core.Execution;
using Core.Execution.Objects;
using Core.Lexing;

namespace Core.Syntax.AST.Expressions.Indexer
{
    public class Index : SyntaxNode
    {
        public Index(SyntaxToken openBracket, Expression expression, SyntaxToken closeBracket)
        {
            OpenBracket = openBracket;
            Expression = expression;
            CloseBracket = closeBracket;
        }

        public SyntaxToken OpenBracket { get; }
        
        public Expression Expression { get; }
        
        public SyntaxToken CloseBracket { get; }

        public override SyntaxToken FirstChild => OpenBracket;

        public override SyntaxToken LastChild => CloseBracket;
        
        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}