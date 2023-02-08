using Core.Execution.DataModel.Objects.Functions;
using Core.Lexing;

namespace Core.Execution.DataModel.Operations.Binary
{
    public class GreaterThanOrEqualOperation : BinaryOperation
    {
        public override TokenType Operator => TokenType.RightArrowEquals;

        public override TokenType? CompoundAssignmentOperator => null;

        public override OperationPrecedence Precedence => OperationPrecedence.Relational;
        
        protected override string MethodName => MagicFunctions.Gte;
    }
}