using Core.Lexing;

namespace Core.Execution.Operations.Unary
{
    public class UnaryMinusOperation : UnaryOperation
    {
        protected override string OperatorMethodName => "op_UnaryNegation";

        protected override TokenTypes Operator => TokenTypes.Minus;
    }
}