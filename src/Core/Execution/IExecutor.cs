using System.Collections.Generic;
using Core.Execution.DataModel.Objects;
using Core.Execution.DataModel.Objects.Functions;
using Core.Syntax.AST;
using Core.Syntax.AST.Expressions;
using Core.Syntax.AST.Expressions.Indexer;
using Core.Syntax.AST.Expressions.Literals;

namespace Core.Execution
{
    public interface IExecutor
    {
        TranslationState<Obj> Execute(SyntaxTree tree);
        
        Obj Execute(ClassStatement statement);

        Obj Execute(FunctionDeclarationStatement statement);
        
        Obj Execute(ReturnStatement statement);

        Obj Execute(BreakStatement statement);
        
        Obj Execute(ContinueStatement statement);
        
        Obj Execute(ForStatement statement);
        
        Obj Execute(WhileStatement statement);
        
        Obj Execute(IfStatement statement);

        Obj Execute(ElseClause clause);

        Obj Execute(BlockStatement block);
        
        Obj Execute(ExpressionStatement statement);

        Obj Execute(VariableAssignmentExpression assignment);

        Obj Execute(IndexAssignmentExpression assignment);
            
        Obj Execute(ParenthesizedExpression expression);

        Obj Execute(IndexAccessExpression expression);
        
        Obj Execute(Index index);

        Obj Execute(BinaryExpression binary);

        Obj Execute(UnaryExpression unary);

        Obj Execute(NumberExpression number);

        Obj Execute(BooleanExpression boolean);

        Obj Execute(StringExpression str);

        Obj Execute(ArrayExpression array);

        Obj Execute(NullExpression expression);

        Obj Execute(VariableExpression variable);

        Obj Execute(CallExpression call);
        
        Obj Execute(TernaryExpression expression);
        
        Obj Execute(AttributeAccessExpression expression);
        
        Obj Execute(AttributeAssignmentExpression assignment);

        Obj InvokeCallableObject(Function callable, IDictionary<string, Obj> arguments);
    }
}