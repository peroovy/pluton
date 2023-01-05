﻿using Core.Execution;
using Core.Execution.Objects;
using Core.Lexing;
using Core.Syntax.AST.Expressions;

namespace Core.Syntax.AST
{
    public class IfStatement : Statement
    {
        public IfStatement(SyntaxToken keyword, Expression condition, Statement thenStatement, ElseClause elseClause)
        {
            Keyword = keyword;
            Condition = condition;
            ThenStatement = thenStatement;
            ElseClause = elseClause;
        }
        
        public SyntaxToken Keyword { get; }
        
        public Expression Condition { get; }
        
        public Statement ThenStatement { get; }
        
        public ElseClause ElseClause { get; }

        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}