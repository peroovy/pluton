using Core.Execution.DataModel.Objects.Functions;
using Core.Lexing;

namespace Core.Execution.DataModel.Operations.Unary
{
    public class UnaryMinusOperation : UnaryOperation
    {
        protected override string MethodName => MagicFunctions.Neg;

        public override TokenType Operator => TokenType.Minus;
    }
}