using System.Collections.Generic;
using Core.Execution;
using Core.Execution.Objects;
using Core.Utils.Text;

namespace Core.Syntax.AST.Expressions
{
    public class IndexAccessExpression : Expression
    {
        public IndexAccessExpression(SourceText sourceText, Expression indexedExpression, Index index)
            : base(sourceText)
        {
            IndexedExpression = indexedExpression;
            Index = index;
        }
        
        public Expression IndexedExpression { get; }
        
        public Index Index { get; }
        
        public override Obj Accept(IExecutor executor) => executor.Execute(this);
        
        public override IEnumerable<Location> GetChildrenLocations()
        {
            yield return IndexedExpression.Location;
            yield return Index.Location;
        }
    }
}