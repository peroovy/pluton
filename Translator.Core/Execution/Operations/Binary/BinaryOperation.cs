﻿using System;
using System.Reflection;
using Translator.Core.Execution.Objects;
using Translator.Core.Lexing;

namespace Translator.Core.Execution.Operations.Binary
{
    public abstract class BinaryOperation
    {
        protected abstract string OperatorMethodName { get; }
        
        protected abstract TokenTypes Operator { get; }

        public bool IsOperator(TokenTypes operatorType) => operatorType == Operator;
        
        public BinaryOperationMethod GetMethod(Obj left, Obj right)
        {
            var leftType = left.GetType();
            var method = leftType.GetMethod(OperatorMethodName,
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { leftType, right.GetType() },
                Array.Empty<ParameterModifier>()
            );
            
            return new BinaryOperationMethod(method);
        }
    }
}