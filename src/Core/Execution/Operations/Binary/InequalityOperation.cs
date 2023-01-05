using Core.Lexing;

namespace Core.Execution.Operations.Binary
{
    public class InequalityOperation : BinaryOperation
    {
        public override TokenType Operator => TokenType.ExclamationMarkEquals;

        public override OperationPrecedence Precedence => OperationPrecedence.Equality;

        protected override string MethodName => "op_Inequality";
    }
}