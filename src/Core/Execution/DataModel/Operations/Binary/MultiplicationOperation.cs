using Core.Execution.DataModel.Objects.Functions;
using Core.Lexing;

namespace Core.Execution.DataModel.Operations.Binary
{
    public class MultiplicationOperation : BinaryOperation
    {
        public override TokenType Operator => TokenType.Star;

        public override TokenType? CompoundAssignmentOperator => TokenType.StarEquals;

        public override OperationPrecedence Precedence => OperationPrecedence.Multiplicative;
        
        protected override string MethodName => MagicFunctions.Mult;
    }
}