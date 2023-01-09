using Core.Lexing;

namespace Core.Execution.Operations.Binary
{
    public class ModulusOperation : BinaryOperation
    {
        public override TokenType Operator => TokenType.Percent;

        public override TokenType? CompoundAssignmentOperator => TokenType.PercentEquals;

        public override OperationPrecedence Precedence => OperationPrecedence.Multiplicative;
        
        protected override string MethodName => "op_Modulus";
    }
}