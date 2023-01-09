using System.Collections.Generic;
using Core.Execution;
using Core.Execution.Objects;
using Core.Lexing;
using Core.Utils.Text;

namespace Core.Syntax.AST.Expressions
{
    public class Index : SyntaxNode
    {
        public Index(SourceText sourceText, SyntaxToken openBracket, Expression expression, SyntaxToken closeBracket)
            : base(sourceText)
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