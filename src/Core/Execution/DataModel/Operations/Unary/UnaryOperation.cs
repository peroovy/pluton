using System;
using System.Collections.Immutable;
using System.Reflection;
using Core.Execution.DataModel.Objects;
using Core.Execution.DataModel.Objects.Functions;
using Array = System.Array;

namespace Core.Execution.DataModel.Operations.Unary
{
    public abstract class UnaryOperation : Operation
    {
        private static readonly ImmutableArray<string> DefaultPositionParameters = ImmutableArray.Create("self");

        private static readonly Type Obj = typeof(Obj);

        public override OperationPrecedence Precedence => OperationPrecedence.Unary;

        protected override int PositionParametersCount => 1;

        public Function FindMethod(Obj operand)
        {
            return FindBuiltinOperation(operand) ?? FindOperationInAttributes(operand);
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