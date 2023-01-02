using Core.Lexing;

namespace Core.Execution.Operations.Binary
{
    public class LessCompareOperation : BinaryOperation
    {
        protected override string OperatorMethodName => "op_LessThan";

        protected override TokenTypes Operator => TokenTypes.LeftArrow;
    }
}