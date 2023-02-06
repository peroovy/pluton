using Core.Execution.Objects.DataModel;
using Core.Lexing;

namespace Core.Execution.Operations.Unary
{
    public class UnaryMinusOperation : UnaryOperation
    {
        protected override string MethodName => MagicFunctions.Neg;

        public override TokenType Operator => TokenType.Minus;
    }
}