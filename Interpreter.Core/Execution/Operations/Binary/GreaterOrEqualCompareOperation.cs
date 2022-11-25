using Interpreter.Core.Lexing;

namespace Interpreter.Core.Execution.Operations.Binary
{
    public class GreaterOrEqualCompareOperation : BinaryOperation
    {
        protected override string OperatorMethodName => "op_GreaterThanOrEqual";

        protected override TokenTypes Operator => TokenTypes.RightArrowEquals;
    }
}