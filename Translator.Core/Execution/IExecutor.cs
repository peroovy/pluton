using Translator.Core.Syntax.AST;

namespace Translator.Core.Execution
{
    public interface IExecutor
    {
        object Execute(ParenthesizedExpression expression);
        
        object Execute(BinaryExpression binary);

        object Execute(NumberExpression number);

        object Execute(BooleanExpression boolean);
    }
}