using Core.Lexing;

namespace Core.Execution.Operations.Binary
{
    public class DivisionOperation : BinaryOperation
    {
        public override TokenType Operator => TokenType.Slash;

        public override TokenType? CompoundAssignmentOperator => TokenType.SlashEquals;

        public override OperationPrecedence Precedence => OperationPrecedence.Multiplicative;

        protected override string MethodName => "op_Division";
    }
}