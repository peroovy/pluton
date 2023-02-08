using Core.Execution.DataModel.Objects.Functions;
using Core.Lexing;

namespace Core.Execution.DataModel.Operations.Binary
{
    public class EqualityOperation : BinaryOperation
    {
        public override TokenType Operator => TokenType.DoubleEquals;

        public override TokenType? CompoundAssignmentOperator => null;

        public override OperationPrecedence Precedence => OperationPrecedence.Equality;

        protected override string MethodName => MagicFunctions.Eq;
    }
}