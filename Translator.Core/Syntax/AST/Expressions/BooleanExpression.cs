﻿using Translator.Core.Execution;
using Translator.Core.Execution.Objects;

namespace Translator.Core.Syntax.AST.Expressions
{
    public class BooleanExpression : Expression
    {
        public BooleanExpression(bool value)
        {
            Value = value;
        }
        
        public bool Value { get; }
        
        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}