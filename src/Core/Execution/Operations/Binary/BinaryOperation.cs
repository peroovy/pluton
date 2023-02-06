using System.Collections.Immutable;
using System.Reflection;
using Core.Execution.Objects;
using Core.Execution.Operations.Unary;
using Core.Lexing;
using Array = System.Array;

namespace Core.Execution.Operations.Binary
{
    public abstract class BinaryOperation : Operation
    {
        private static readonly ImmutableArray<string> DefaultPositionParameters =
            ImmutableArray.Create("self", "other");
        
        protected override int PositionParametersCount => 2;
        
        public abstract TokenType? CompoundAssignmentOperator { get; }
        
        public Function FindOperation(Obj left, Obj right)
        {
            return FindBuiltinOperation(left, right) ?? FindOperationInAttributes(left);
        }
        
        private BuiltinOperationWrapper FindBuiltinOperation(Obj left, Obj right)
        {
            var leftType = left.GetType();
            
            var method = leftType.GetMethod(
                MethodName,
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { leftType, right.GetType() },
                Array.Empty<ParameterModifier>()
            );

            return method is null
                ? null
                : new BuiltinOperationWrapper(
                    MethodName,
                    DefaultPositionParameters,
                    () => (Obj)method.Invoke(null, new object[] { left, right })
                );
        }
    }
}