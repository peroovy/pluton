using System.Collections.Generic;
using System.Linq;
using Core.Execution;
using Core.Execution.Objects;
using Core.Utils.Text;

namespace Core.Syntax.AST
{
    public abstract class SyntaxNode
    {
        protected SyntaxNode(SourceText sourceText)
        {
            SourceText = sourceText;
        }
        
        public TextSpan Span
        {
            get
            {
                var first = GetChildrenLocations().First().Span;
                var last = GetChildrenLocations().Last().Span;

                return first == last 
                    ? first 
                    : new TextSpan(first.Start, last.Start - first.Start + 1);
            }
        }

        public Location Location => new(SourceText, Span);

        public SourceText SourceText { get; }

        public abstract Obj Accept(IExecutor executor);

        public abstract IEnumerable<Location> GetChildrenLocations();

        public override string ToString() => SourceText.Value.Substring(Span.Start, Span.Length);
    }
}