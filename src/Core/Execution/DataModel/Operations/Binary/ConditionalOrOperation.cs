using Core.Execution.DataModel.Objects.Functions;
using Core.Lexing;

namespace Core.Execution.DataModel.Operations.Binary
{
    public class ConditionalOrOperation : BinaryOperation
    {
        public override TokenType Operator => TokenType.OrKeyword;

        public override TokenType? CompoundAssignmentOperator => null;

        public override OperationPrecedence Precedence => OperationPrecedence.ConditionalOr;
        
        protected override string MethodName => MagicFunctions.Or;
    }
}