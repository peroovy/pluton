using System;
using System.Reflection;
using Core.Execution.Objects;
using Core.Lexing;
using Array = System.Array;

namespace Core.Execution.Operations.Binary
{
    public abstract class BinaryOperation
    {
        private static Type objType = typeof(Obj);
        
        public abstract TokenType Operator { get; }
        
        public abstract TokenType? CompoundAssignmentOperator { get; }
        
        public abstract OperationPrecedence Precedence { get; }
        
        protected abstract string MethodName { get; }

        public bool IsOperator(TokenType tokenType) => tokenType == Operator;
        
        public Func<Obj> FindMethod(Obj left, Obj right)
        {
            var method = FindMethod(left.GetType(), right.GetType()) ?? FindMethod(objType, objType);

            if (method is null)
                return null;

            return () => (Obj)method.Invoke(null, new object[] { left, right });
        }

        private MethodInfo FindMethod(Type leftType, Type rightType)
        {
            return leftType.GetMethod(MethodName,
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { leftType, rightType },
                Array.Empty<ParameterModifier>()
            );
        }
    }
}