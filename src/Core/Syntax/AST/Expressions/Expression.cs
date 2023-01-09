using Core.Utils.Text;

namespace Core.Syntax.AST.Expressions
{
    public abstract class Expression : SyntaxNode
    {
        protected Expression(SourceText sourceText) : base(sourceText)
        {
        }
    }
}