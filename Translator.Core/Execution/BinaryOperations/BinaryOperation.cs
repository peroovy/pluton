using System;
using System.Reflection;
using Translator.Core.Execution.Objects;
using Translator.Core.Lexing;

namespace Translator.Core.Execution.BinaryOperations
{
    public abstract class BinaryOperation
    {
        protected abstract string OperatorMethodName { get; }
        
        protected abstract TokenTypes Operator { get; }

        public bool IsOperator(TokenTypes operatorType) => operatorType == Operator;
        
        public MethodInfo GetMethod(Obj left, Obj right)
        {
            var leftType = left.GetType();
            var operation = leftType.GetMethod(OperatorMethodName,
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { leftType, right.GetType() },
                Array.Empty<ParameterModifier>()
            );
            
            return operation;
        }

        public static Obj Evaluate(MethodInfo operationMethod, Obj left, Obj right)
        {
            return operationMethod is null
                ? new Undefined()
                : (Obj)operationMethod.Invoke(null, new object[] {left, right});
        }
    }
}