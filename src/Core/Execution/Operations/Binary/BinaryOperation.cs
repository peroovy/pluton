using System;
using System.Reflection;
using Core.Execution.Objects;
using Core.Lexing;
using Array = System.Array;

namespace Core.Execution.Operations.Binary
{
    public abstract class BinaryOperation
    {
        public abstract TokenType Operator { get; }
        
        public abstract TokenType? CompoundAssignmentOperator { get; }
        
        public abstract OperationPrecedence Precedence { get; }
        
        protected abstract string MethodName { get; }

        public bool IsOperator(TokenType tokenType) => tokenType == Operator;
        
        public Func<Obj> FindMethod(Obj left, Obj right)
        {
            var leftType = left.GetType();
            var method = leftType.GetMethod(MethodName,
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { leftType, right.GetType() },
                Array.Empty<ParameterModifier>()
            );

            if (method is null)
                return null;

            return () => (Obj)method.Invoke(null, new object[] { left, right });
        }
    }
}