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
        
        public SyntaxToken Token { get; }

        public override IEnumerable<Location> GetChildrenLocations()
        {
            yield return Token.Location;
        }
    }
}