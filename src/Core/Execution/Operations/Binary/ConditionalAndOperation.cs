using Core.Execution.Objects.DataModel;
using Core.Lexing;

namespace Core.Execution.Operations.Binary
{
    public class ConditionalAndOperation : BinaryOperation
    {
        public override TokenType Operator => TokenType.AndKeyword;

        public override TokenType? CompoundAssignmentOperator => null;

        public override OperationPrecedence Precedence => OperationPrecedence.ConditionalAnd;

        protected override string MethodName => MagicFunctions.And;
    }
}