using Core.Execution.DataModel.Objects.Functions;
using Core.Lexing;

namespace Core.Execution.DataModel.Operations.Unary
{
    public class UnaryPlusOperation : UnaryOperation
    {
        protected override string MethodName => MagicFunctions.Pos;

        public override TokenType Operator => TokenType.Plus;
    }
}