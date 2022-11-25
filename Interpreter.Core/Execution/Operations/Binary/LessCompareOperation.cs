using Interpreter.Core.Lexing;

namespace Interpreter.Core.Execution.Operations.Binary
{
    public class LessCompareOperation : BinaryOperation
    {
        protected override string OperatorMethodName => "op_LessThan";

        protected override TokenTypes Operator => TokenTypes.LeftArrow;
    }
}