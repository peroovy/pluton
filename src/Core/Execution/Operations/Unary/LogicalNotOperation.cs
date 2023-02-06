using Core.Execution.Objects.DataModel;
using Core.Lexing;

namespace Core.Execution.Operations.Unary
{
    public class LogicalNotOperation : UnaryOperation
    {
        protected override string MethodName => MagicFunctions.Not;

        public override TokenType Operator => TokenType.NotKeyword;
    }
}