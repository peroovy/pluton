using Translator.Core.Lexing;

namespace Translator.Core.Execution.Operations.Binary
{
    public class LessCompareOperation : BinaryOperation
    {
        protected override string OperatorMethodName => "op_LessThan";

        protected override TokenTypes Operator => TokenTypes.LeftArrow;
    }
}