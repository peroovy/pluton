using System;
using Core.Execution;
using Core.Execution.DataModel.Objects;
using Core.Lexing;
using Core.Utils.Text;

namespace Core.Syntax.AST
{
    public abstract class SyntaxNode
    {
        public TextSpan Span
        {
            get
            {
                var first = FirstChild.Location.Span;
                var last = LastChild.Location.Span;

                return new TextSpan(first.Start, last.End - first.Start);
            }
        }

        public Location Location
        {
            get
            {
                if (FirstChild.Location.SourceText != LastChild.Location.SourceText)
                    throw new InvalidOperationException("Children are from different source codes");
                
                return new Location(FirstChild.Location.SourceText, Span);
            }
        }

        public abstract SyntaxToken FirstChild { get; }
        
        public abstract SyntaxToken LastChild { get; }

        public abstract Obj Accept(IExecutor executor);
    }
}