using Core.Lexing;

namespace Core.Execution.Operations.Binary
{
    public class SubtractionOperation : BinaryOperation
    {
        public override TokenType Operator => TokenType.Minus;

        public override OperationPrecedence Precedence => OperationPrecedence.Additive;
        
        protected override string MethodName => "op_Subtraction";
    }
}