using System.Collections.Generic;
using Core.Execution;
using Core.Execution.Objects;
using Core.Lexing;
using Core.Utils.Text;

namespace Core.Syntax.AST.Expressions
{
    public class IndexAssignmentExpression : Expression
    {
        public IndexAssignmentExpression(
            SourceText sourceText,
            Expression indexedExpression, 
            Index index, 
            SyntaxToken equalsToken, 
            Expression value) : base(sourceText)
        {
            IndexedExpression = indexedExpression;
            Index = index;
            EqualsToken = equalsToken;
            Value = value;
        }
        
        public Expression IndexedExpression { get; }
        
        public Index Index { get; }
        
        public SyntaxToken EqualsToken { get; }

        public Expression Value { get; }

        public override Obj Accept(IExecutor executor) => executor.Execute(this);
        
        public override IEnumerable<Location> GetChildrenLocations()
        {
            yield return IndexedExpression.Location;
            yield return Index.Location;
            yield return EqualsToken.Location;
            yield return Value.Location;
        }
    }
}