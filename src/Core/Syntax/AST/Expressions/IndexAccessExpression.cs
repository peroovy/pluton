using System.Collections.Generic;
using Core.Execution;
using Core.Execution.Objects;
using Core.Lexing;
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

        public override SyntaxToken FirstChild => IndexedExpression.FirstChild;

        public override SyntaxToken LastChild => Index.LastChild;
        
        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}