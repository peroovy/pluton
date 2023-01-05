﻿using Core.Execution;
using Core.Execution.Objects;
using Core.Lexing;
using Core.Syntax.AST.Expressions;

namespace Core.Syntax.AST
{
    public class ExpressionStatement : Statement
    {
        public ExpressionStatement(Expression expression, SyntaxToken semicolon)
        {
            Expression = expression;
            Semicolon = semicolon;
        }
        
        public Expression Expression { get; }
        
        public SyntaxToken Semicolon { get; }

        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}