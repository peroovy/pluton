using Core.Lexing;

namespace Core.Execution.Operations.Binary
{
    public class MultiplicationOperation : BinaryOperation
    {
        public override TokenType Operator => TokenType.Star;

        public override OperationPrecedence Precedence => OperationPrecedence.Multiplicative;
        
        protected override string MethodName => "op_Multiply";
    }
}