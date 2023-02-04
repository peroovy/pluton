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
            var method = FindMethod(operand.GetType());

            if (method is null)
                return null;
            
            return () => (Obj)method.Invoke(null, new object[] { operand });
        }

        private MethodInfo FindMethod(Type operandType)
        {
            return operandType.GetMethod(MethodName,
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { operandType },
                Array.Empty<ParameterModifier>()
            );
        }
    }
}