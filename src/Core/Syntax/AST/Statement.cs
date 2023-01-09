using Core.Utils.Text;

namespace Core.Syntax.AST
{
    public abstract class Statement : SyntaxNode
    {
        protected Statement(SourceText sourceText) : base(sourceText)
        {
        }
    }
}