using System;
using System.Reflection;
using Translator.Core.Execution.Objects;
using Translator.Core.Lexing;

namespace Translator.Core.Execution.Operations.Unary
{
    public abstract class UnaryOperation
    {
        protected abstract string OperatorMethodName { get; }
        
        protected abstract TokenTypes Operator { get; }

        public bool IsOperator(TokenTypes operatorType) => operatorType == Operator;
        
        public UnaryOperationMethod GetMethod(Obj operand)
        {
            var type = operand.GetType();
            var method = type.GetMethod(OperatorMethodName,
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { type },
                Array.Empty<ParameterModifier>()
            );
            
            return new UnaryOperationMethod(method);
        }
    }
}