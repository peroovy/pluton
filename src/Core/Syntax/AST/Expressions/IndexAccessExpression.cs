using Core.Execution;
using Core.Execution.Objects;

namespace Core.Syntax.AST.Expressions
{
    public class IndexAccessExpression : Expression
    {
        public IndexAccessExpression(Expression indexedExpression, SyntaxIndex index)
        {
            IndexedExpression = indexedExpression;
            Index = index;
        }
        
        public Expression IndexedExpression { get; }
        
        public SyntaxIndex Index { get; }
        
        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}