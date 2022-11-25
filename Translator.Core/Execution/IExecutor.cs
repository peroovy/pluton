using Translator.Core.Execution.Objects;
using Translator.Core.Syntax.AST;
using Translator.Core.Syntax.AST.Expressions;

namespace Translator.Core.Execution
{
    public interface IExecutor
    {
        Obj Execute(FunctionDeclarationStatement statement);

        Obj Execute(ReturnStatement statement);
        
        Obj Execute(ForStatement statement);
        
        Obj Execute(WhileStatement statement);
        
        Obj Execute(IfStatement statement);

        Obj Execute(ElseClause clause);

        Obj Execute(BlockStatement block);
        
        Obj Execute(ExpressionStatement statement);
        
        Obj Execute(AssignmentExpression assignment);
            
        Obj Execute(ParenthesizedExpression expression);
        
        Obj Execute(BinaryExpression binary);

        Obj Execute(UnaryExpression unary);

        Obj Execute(NumberExpression number);

        Obj Execute(BooleanExpression boolean);

        Obj Execute(VariableExpression variable);

        Obj Execute(FunctionCallExpression call);
    }
}