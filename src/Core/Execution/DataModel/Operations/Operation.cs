using Core.Execution.DataModel.Objects;
using Core.Execution.DataModel.Objects.Functions;
using Core.Lexing;

namespace Core.Execution.DataModel.Operations
{
    public abstract class Operation
    {
        public abstract TokenType Operator { get; }

        public abstract OperationPrecedence Precedence { get; }

        protected abstract string MethodName { get; }

        protected abstract int PositionParametersCount { get; }

        public bool IsOperator(TokenType tokenType)
        {
            return tokenType == Operator;
        }

        protected Function FindOperationInAttributes(Obj obj)
        {
            if (!obj.TryGetAttributeFromBaseClass(MethodName, out var attr))
                return null;

            if (attr is not Function function || function.PositionParameters.Length != PositionParametersCount)
                return null;

            return function;
        }
    }
}