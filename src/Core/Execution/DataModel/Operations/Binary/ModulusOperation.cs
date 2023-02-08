using Core.Execution.DataModel.Objects.Functions;
using Core.Lexing;

namespace Core.Execution.DataModel.Operations.Binary
{
    public class ModulusOperation : BinaryOperation
    {
        public override TokenType Operator => TokenType.Percent;

        public override TokenType? CompoundAssignmentOperator => TokenType.PercentEquals;

        public override OperationPrecedence Precedence => OperationPrecedence.Multiplicative;
        
        protected override string MethodName => MagicFunctions.Mod;
    }
}