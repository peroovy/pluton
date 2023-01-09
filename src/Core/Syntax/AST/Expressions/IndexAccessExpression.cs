using Core.Execution;
using Core.Execution.Objects;
using Core.Lexing;

namespace Core.Syntax.AST.Expressions
{
    public class IndexAccessExpression : Expression
    {
        public IndexAccessExpression(Expression indexedExpression, Index index)
        {
            IndexedExpression = indexedExpression;
            Index = index;
        }
        
        public Expression IndexedExpression { get; }
        
        public Index Index { get; }

        public override SyntaxToken FirstChild => IndexedExpression.FirstChild;

        public override SyntaxToken LastChild => Index.LastChild;
        
        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}