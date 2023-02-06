using Core.Execution.Objects.DataModel;
using Core.Lexing;

namespace Core.Execution.Operations.Binary
{
    public class GreaterThanOperation : BinaryOperation
    {
        public override TokenType Operator => TokenType.RightArrow;

        public override TokenType? CompoundAssignmentOperator => null;

        public override OperationPrecedence Precedence => OperationPrecedence.Relational;

        protected override string MethodName => MagicFunctions.Gt;
    }
}