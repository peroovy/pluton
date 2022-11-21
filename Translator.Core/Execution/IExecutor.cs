using Translator.Core.Syntax.AST;
using Translator.Core.Syntax.AST.Expressions;

namespace Translator.Core.Execution
{
    public interface IExecutor
    {
        Object Execute(ExpressionStatement statement);
        
        Object Execute(AssignmentExpression assignment);
            
        Object Execute(ParenthesizedExpression expression);
        
        Object Execute(BinaryExpression binary);

        Object Execute(NumberExpression number);

        Object Execute(BooleanExpression boolean);

        Object Execute(VariableExpression variable);
    }
}