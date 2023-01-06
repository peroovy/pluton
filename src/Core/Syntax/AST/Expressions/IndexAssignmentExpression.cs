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

        public override SyntaxToken FirstChild => IndexedExpression.FirstChild;

        public override SyntaxToken LastChild => Value.LastChild;
        
        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}