using System.Collections.Immutable;
using System.Reflection;
using Core.Execution.Objects;
using Array = System.Array;

namespace Core.Execution.Operations.Unary
{
    public abstract class UnaryOperation : Operation
    {
        private static readonly ImmutableArray<string> DefaultPositionParameters = ImmutableArray.Create("self");
        
        public override OperationPrecedence Precedence => OperationPrecedence.Unary;

        protected override int PositionParametersCount => 1;

        public Function FindMethod(Obj operand)
        {
            return FindBuiltinMethod(operand) ?? FindOperationInAttributes(operand);
        }

        private BuiltinOperationWrapper FindBuiltinMethod(Obj operand)
        {
            var operandType = operand.GetType();
            
            var method = operandType.GetMethod(MethodName,
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { operandType },
                Array.Empty<ParameterModifier>()
            );

            return method is null
                ? null
                : new BuiltinOperationWrapper(
                    MethodName,
                    DefaultPositionParameters,
                    () => (Obj)method.Invoke(null, new object[] { operand })
                );
        }
    }
}