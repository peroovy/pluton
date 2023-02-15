using System;
using System.Collections.Immutable;
using System.Reflection;
using Core.Execution.DataModel.Objects;
using Core.Execution.DataModel.Objects.Functions;
using Core.Lexing;
using Array = System.Array;

namespace Core.Execution.DataModel.Operations.Binary
{
    public abstract class BinaryOperation
    {
        private static readonly ImmutableArray<string> DefaultPositionParameters =
            ImmutableArray.Create("self", "other");

        private static readonly Type Obj = typeof(Obj);

        public abstract TokenType Operator { get; }
        
        public abstract TokenType? CompoundAssignmentOperator { get; }

        public abstract OperationPrecedence Precedence { get; }
    
        protected abstract string MethodName { get; }
        
        public bool IsOperator(TokenType tokenType)
        {
            return tokenType == Operator;
        }
        
        public Function FindOperation(Obj left, Obj right)
        {
            var builtin = FindBuiltinOperation(left, right);
            if (builtin is not null)
                return builtin;

            return left.FindMethod(MethodName, 2, out var method)
                ? method
                : null;
        }

        private BuiltinOperationWrapper FindBuiltinOperation(Obj left, Obj right)
        {
            var method = FindBuiltinMethod(left.GetType(), right.GetType()) ?? FindBuiltinMethod(Obj, Obj);

            return method is null
                ? null
                : new BuiltinOperationWrapper(
                    MethodName,
                    DefaultPositionParameters,
                    () => (Obj)method.Invoke(null, new object[] { left, right })
                );
        }

        private MethodInfo FindBuiltinMethod(Type left, Type right)
        {
            return left.GetMethod(
                MethodName,
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { left, right },
                Array.Empty<ParameterModifier>()
            );
        }
    }
}