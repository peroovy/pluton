using Core.Execution.Objects.DataModel;
using Core.Lexing;

namespace Core.Execution.Operations.Binary
{
    public class ConditionalOrOperation : BinaryOperation
    {
        public override TokenType Operator => TokenType.OrKeyword;

        public override TokenType? CompoundAssignmentOperator => null;

        public override OperationPrecedence Precedence => OperationPrecedence.ConditionalOr;
        
        protected override string MethodName => MagicFunctions.Or;
    }
}