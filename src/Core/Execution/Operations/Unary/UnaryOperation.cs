using System;
using System.Reflection;
using Core.Execution.Objects;
using Core.Lexing;
using Array = System.Array;

namespace Core.Execution.Operations.Unary
{
    public abstract class UnaryOperation
    {
        public abstract TokenType Operator { get; }

        public virtual OperationPrecedence Precedence => OperationPrecedence.Unary;

        protected abstract string MethodName { get; }
        
        public bool IsOperator(TokenType tokenType) => tokenType == Operator;
        
        public Func<Obj> FindMethod(Obj operand)
        {
            var type = operand.GetType();
            var method = type.GetMethod(MethodName,
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { type },
                Array.Empty<ParameterModifier>()
            );

            if (method is null)
                return null;
            
            return () => (Obj)method.Invoke(null, new object[] { operand });
        }
    }
}