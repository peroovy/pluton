using Core.Execution.DataModel.Objects.Functions;
using Core.Lexing;

namespace Core.Execution.DataModel.Operations.Binary
{
    public class AdditionOperation : BinaryOperation
    {
        public override TokenType Operator => TokenType.Plus;

        public override TokenType? CompoundAssignmentOperator => TokenType.PlusEquals;

        public override OperationPrecedence Precedence => OperationPrecedence.Additive;
        
        protected override string MethodName => MagicFunctions.Add;
    }
}