using Core.Lexing;

namespace Core.Execution.Operations.Binary
{
    public class EqualityOperation : BinaryOperation
    {
        public override TokenType Operator => TokenType.DoubleEquals;

        public override OperationPrecedence Precedence => OperationPrecedence.Equality;

        protected override string MethodName => "op_Equality";
    }
}