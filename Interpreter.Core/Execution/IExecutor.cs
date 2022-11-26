﻿using Interpreter.Core.Execution.Objects;
using Interpreter.Core.Syntax.AST;
using Interpreter.Core.Syntax.AST.Expressions;

namespace Interpreter.Core.Execution
{
    public interface IExecutor
    {
        void Execute(SyntaxTree tree);
        
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