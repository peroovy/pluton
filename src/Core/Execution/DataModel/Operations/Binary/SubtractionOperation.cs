using Core.Execution.DataModel.Objects.Functions;
using Core.Lexing;

namespace Core.Execution.DataModel.Operations.Binary
{
    public class SubtractionOperation : BinaryOperation
    {
        public override TokenType Operator => TokenType.Minus;

        public override TokenType? CompoundAssignmentOperator => TokenType.MinusEquals;

        public override OperationPrecedence Precedence => OperationPrecedence.Additive;
        
        protected override string MethodName => MagicFunctions.Sub;
    }
}