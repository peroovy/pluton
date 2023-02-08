using Core.Execution.DataModel.Objects.Functions;
using Core.Lexing;

namespace Core.Execution.DataModel.Operations.Binary
{
    public class DivisionOperation : BinaryOperation
    {
        public override TokenType Operator => TokenType.Slash;

        public override TokenType? CompoundAssignmentOperator => TokenType.SlashEquals;

        public override OperationPrecedence Precedence => OperationPrecedence.Multiplicative;

        protected override string MethodName => MagicFunctions.Div;
    }
}