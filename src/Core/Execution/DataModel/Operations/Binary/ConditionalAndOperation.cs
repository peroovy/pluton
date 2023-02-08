using Core.Execution.DataModel.Objects.Functions;
using Core.Lexing;

namespace Core.Execution.DataModel.Operations.Binary
{
    public class ConditionalAndOperation : BinaryOperation
    {
        public override TokenType Operator => TokenType.AndKeyword;

        public override TokenType? CompoundAssignmentOperator => null;

        public override OperationPrecedence Precedence => OperationPrecedence.ConditionalAnd;

        protected override string MethodName => MagicFunctions.And;
    }
}