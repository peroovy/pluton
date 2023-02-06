using Core.Execution.Objects.DataModel;
using Core.Lexing;

namespace Core.Execution.Operations.Unary
{
    public class UnaryPlusOperation : UnaryOperation
    {
        protected override string MethodName => MagicFunctions.Pos;

        public override TokenType Operator => TokenType.Plus;
    }
}