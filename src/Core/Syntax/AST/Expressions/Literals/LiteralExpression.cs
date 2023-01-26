using Core.Lexing;

namespace Core.Syntax.AST.Expressions.Literals
{
    public abstract class LiteralExpression : Expression
    {
        protected LiteralExpression(SyntaxToken token)
        {
            Token = token;
        }

        public override SyntaxToken FirstChild => Token;

        public override SyntaxToken LastChild => Token;

        public SyntaxToken Token { get; }
    }
}