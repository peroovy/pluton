using Core.Lexing;

namespace Core.Execution.Operations.Unary
{
    public class UnaryPlusOperation : UnaryOperation
    {
        protected override string OperatorMethodName => "op_UnaryPlus";

        protected override TokenTypes Operator => TokenTypes.Plus;
    }
}