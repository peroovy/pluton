using Core.Execution.DataModel.Objects.Functions;
using Core.Lexing;

namespace Core.Execution.DataModel.Operations.Binary
{
    public class InequalityOperation : BinaryOperation
    {
        public override TokenType Operator => TokenType.ExclamationMarkEquals;

        public override TokenType? CompoundAssignmentOperator => null;

        public override OperationPrecedence Precedence => OperationPrecedence.Equality;

        protected override string MethodName => MagicFunctions.Neq;
    }
}