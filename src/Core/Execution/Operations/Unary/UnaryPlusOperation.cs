using Core.Lexing;

namespace Core.Execution.Operations.Unary
{
    public class UnaryPlusOperation : UnaryOperation
    {
        protected override string MethodName => "op_UnaryPlus";

        public override TokenType Operator => TokenType.Plus;
    }
}