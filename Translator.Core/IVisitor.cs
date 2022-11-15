using Translator.Core.Syntax.AST;

namespace Translator.Core
{
    public interface IVisitor<out T>
    {
        T Visit(ParenthesizedExpression expression);
        
        T Visit(BinaryExpression binary);

        T Visit(NumberExpression number);
    }
}