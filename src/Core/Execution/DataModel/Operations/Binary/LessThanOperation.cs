using Core.Execution.DataModel.Objects.Functions;
using Core.Lexing;

namespace Core.Execution.DataModel.Operations.Binary
{
    public class LessThanOperation : BinaryOperation
    {
        public override TokenType Operator => TokenType.LeftArrow;

        public override TokenType? CompoundAssignmentOperator => null;

        public override OperationPrecedence Precedence => OperationPrecedence.Relational;

        protected override string MethodName => MagicFunctions.Lt;
    }
}