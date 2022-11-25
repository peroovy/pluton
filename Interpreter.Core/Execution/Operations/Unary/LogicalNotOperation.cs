using Interpreter.Core.Lexing;

namespace Interpreter.Core.Execution.Operations.Unary
{
    public class LogicalNotOperation : UnaryOperation
    {
        protected override string OperatorMethodName => "op_LogicalNot";

        protected override TokenTypes Operator => TokenTypes.ExclamationMark;
    }
}