﻿using System.Collections.Immutable;
using Core.Execution;
using Core.Execution.Objects;
using Core.Lexing;

namespace Core.Syntax.AST.Expressions
{
    public class FunctionCallExpression : Expression
    {
        public FunctionCallExpression(
            SyntaxToken identifier, 
            SyntaxToken openParenthesis, 
            ImmutableArray<Expression> arguments, 
            SyntaxToken closeParenthesis)
        {
            Identifier = identifier;
            OpenParenthesis = openParenthesis;
            Arguments = arguments;
            CloseParenthesis = closeParenthesis;
        }
        
        public SyntaxToken Identifier { get; }
        
        public SyntaxToken OpenParenthesis { get; }
        
        public ImmutableArray<Expression> Arguments { get; }
        
        public SyntaxToken CloseParenthesis { get; }
        
        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}