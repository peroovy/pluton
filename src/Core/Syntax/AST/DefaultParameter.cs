﻿using System.Collections.Generic;
using Core.Execution;
using Core.Execution.Objects;
using Core.Lexing;
using Core.Syntax.AST.Expressions;
using Core.Utils.Text;

namespace Core.Syntax.AST
{
    public class DefaultParameter : SyntaxNode
    {
        public DefaultParameter(SourceText sourceText, SyntaxToken name, SyntaxToken equalsToken, Expression expression)
            : base(sourceText)
        {
            Name = name;
            EqualsToken = equalsToken;
            Expression = expression;
        }
        
        public SyntaxToken Name { get; }
        
        public SyntaxToken EqualsToken { get; }
        
        public Expression Expression { get; }

        public override SyntaxToken FirstChild => Name;

        public override SyntaxToken LastChild => Expression.LastChild;
        
        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}