using System.Collections.Generic;
using System.Linq;
using Core.Execution;
using Core.Execution.Objects;
using Core.Lexing;
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
                var first = FirstChild.Location.Span;
                var last = LastChild.Location.Span;

                return new TextSpan(first.Start, last.End - first.Start);
            }
        }

        public Location Location => new(SourceText, Span);

        public SourceText SourceText { get; }
        
        public abstract SyntaxToken FirstChild { get; }
        
        public abstract SyntaxToken LastChild { get; }

        public abstract Obj Accept(IExecutor executor);

        public override string ToString() => SourceText.Value.Substring(Span.Start, Span.Length);
    }
}