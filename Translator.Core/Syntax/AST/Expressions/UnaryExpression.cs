﻿using Translator.Core.Execution;
using Translator.Core.Execution.Objects;
using Translator.Core.Lexing;

namespace Translator.Core.Syntax.AST.Expressions
{
    public class UnaryExpression : Expression
    {
        public UnaryExpression(SyntaxToken operatorToken, Expression operand)
        {
            OperatorToken = operatorToken;
            Operand = operand;
        }
        
        public SyntaxToken OperatorToken { get; }
        
        public Expression Operand { get; }
        
        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}