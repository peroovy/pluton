using Translator.Core.Lexing;

namespace Translator.Core.Execution.Operations.Binary
{
    public class GreaterCompareOperation : BinaryOperation
    {
        protected override string OperatorMethodName => "op_GreaterThan";

        protected override TokenTypes Operator => TokenTypes.RightArrow;
    }
}