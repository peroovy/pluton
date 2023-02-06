using Core.Execution.Objects;
using Core.Lexing;

namespace Core.Execution.Operations
{
    public abstract class Operation
    {
        public abstract TokenType Operator { get; }

        public abstract OperationPrecedence Precedence { get; }

        protected abstract string MethodName { get; }
        
        protected abstract int PositionParametersCount { get; }
        
        public bool IsOperator(TokenType tokenType) => tokenType == Operator;

        protected Function FindOperationInAttributes(Obj obj)
        {
            // TODO: fix BaseClass may be null
            if (!obj.BaseClass.TryGetAttribute(MethodName, out var attr))
                return null;

            if (attr is not Function function || function.PositionParameters.Length != PositionParametersCount)
                return null;

            return function;
        }
    }
}