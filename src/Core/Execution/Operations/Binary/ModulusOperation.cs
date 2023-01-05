using Core.Lexing;

namespace Core.Execution.Operations.Binary
{
    public class ModulusOperation : BinaryOperation
    {
        public override TokenType Operator => TokenType.Percent;

        public override OperationPrecedence Precedence => OperationPrecedence.Multiplicative;
        
        protected override string MethodName => "op_Modulus";
    }
}