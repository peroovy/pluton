using Core.Execution.DataModel.Objects.Functions;
using Core.Lexing;

namespace Core.Execution.DataModel.Operations.Unary
{
    public class LogicalNotOperation : UnaryOperation
    {
        protected override string MethodName => MagicFunctions.Not;

        public override TokenType Operator => TokenType.NotKeyword;
    }
}