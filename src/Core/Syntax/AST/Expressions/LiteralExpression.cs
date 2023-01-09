using System.Collections.Generic;
using Core.Lexing;
using Core.Utils.Text;

namespace Core.Syntax.AST.Expressions
{
    public abstract class LiteralExpression : Expression
    {
        protected LiteralExpression(SourceText sourceText, SyntaxToken token) : base(sourceText)
        {
            Token = token;
        }

        public override SyntaxToken FirstChild => Token;

        public override SyntaxToken LastChild => Token;

        public SyntaxToken Token { get; }
    }
}