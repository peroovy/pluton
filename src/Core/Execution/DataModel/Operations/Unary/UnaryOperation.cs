using System;
using System.Collections.Immutable;
using System.Reflection;
using Core.Execution.DataModel.Objects;
using Core.Execution.DataModel.Objects.Functions;
using Core.Lexing;
using Array = System.Array;

namespace Core.Execution.DataModel.Operations.Unary
{
    public abstract class UnaryOperation
    {
        private static readonly ImmutableArray<string> DefaultPositionParameters = ImmutableArray.Create("self");

        private static readonly Type Obj = typeof(Obj);

        protected abstract string MethodName { get; }
        
        public abstract TokenType Operator { get; }

        public static OperationPrecedence Precedence => OperationPrecedence.Unary;
        
        public bool IsOperator(TokenType tokenType)
        {
            return tokenType == Operator;
        }

        public Function FindOperation(Obj operand)
        {
            var builtin = FindBuiltinOperation(operand);
            if (builtin is not null)
                return builtin;

            return operand.FindMethod(MethodName, 1, out var method)
                ? method
                : null;
        }

        private BuiltinOperationWrapper FindBuiltinOperation(Obj operand)
        {
            var method = FindBuiltinMethod(operand.GetType()) ?? FindBuiltinMethod(Obj);

            return method is null
                ? null
                : new BuiltinOperationWrapper(
                    MethodName,
                    DefaultPositionParameters,
                    () => (Obj)method.Invoke(null, new object[] { operand })
                );
        }

        private MethodInfo FindBuiltinMethod(Type operand)
        {
            return operand.GetMethod(
                MethodName,
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { operand },
                Array.Empty<ParameterModifier>()
            );
        }
    }
}