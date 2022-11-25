using Interpreter.Core.Lexing;

namespace Interpreter.Core.Execution.Operations.Binary
{
    public class GreaterCompareOperation : BinaryOperation
    {
        protected override string OperatorMethodName => "op_GreaterThan";

        protected override TokenTypes Operator => TokenTypes.RightArrow;
    }
}