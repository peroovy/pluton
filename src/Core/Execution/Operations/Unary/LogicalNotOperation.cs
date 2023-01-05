using Core.Lexing;

namespace Core.Execution.Operations.Unary
{
    public class LogicalNotOperation : UnaryOperation
    {
        protected override string MethodName => "op_LogicalNot";

        public override TokenType Operator => TokenType.NotKeyword;
    }
}