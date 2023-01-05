using Core.Lexing;

namespace Core.Execution.Operations.Binary
{
    public class GreaterThanOperation : BinaryOperation
    {
        public override TokenType Operator => TokenType.RightArrow;

        public override OperationPrecedence Precedence => OperationPrecedence.Relational;

        protected override string MethodName => "op_GreaterThan";
    }
}