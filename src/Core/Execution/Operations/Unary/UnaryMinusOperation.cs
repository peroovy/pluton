using Core.Lexing;

namespace Core.Execution.Operations.Unary
{
    public class UnaryMinusOperation : UnaryOperation
    {
        protected override string MethodName => "op_UnaryNegation";

        public override TokenType Operator => TokenType.Minus;
    }
}