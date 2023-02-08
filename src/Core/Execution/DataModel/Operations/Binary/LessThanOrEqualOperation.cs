using Core.Execution.DataModel.Objects.Functions;
using Core.Lexing;

namespace Core.Execution.DataModel.Operations.Binary
{
    public class LessThanOrEqualOperation : BinaryOperation
    {
        public override TokenType Operator => TokenType.LeftArrowEquals;

        public override TokenType? CompoundAssignmentOperator => null;

        public override OperationPrecedence Precedence => OperationPrecedence.Relational;

        protected override string MethodName => MagicFunctions.Lte;
    }
}