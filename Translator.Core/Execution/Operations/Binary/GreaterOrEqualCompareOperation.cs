using Translator.Core.Lexing;

namespace Translator.Core.Execution.Operations.Binary
{
    public class GreaterOrEqualCompareOperation : BinaryOperation
    {
        protected override string OperatorMethodName => "op_GreaterThanOrEqual";

        protected override TokenTypes Operator => TokenTypes.RightArrowEquals;
    }
}