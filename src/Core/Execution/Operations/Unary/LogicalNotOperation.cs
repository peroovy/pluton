using Core.Lexing;

namespace Core.Execution.Operations.Unary
{
    public class LogicalNotOperation : UnaryOperation
    {
        protected override string OperatorMethodName => "op_LogicalNot";

        protected override TokenTypes Operator => TokenTypes.NotKeyword;
    }
}