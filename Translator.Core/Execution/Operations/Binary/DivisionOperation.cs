using Translator.Core.Lexing;

namespace Translator.Core.Execution.Operations.Binary
{
    public class DivisionOperation : BinaryOperation
    {
        protected override string OperatorMethodName => "op_Division";

        protected override TokenTypes Operator => TokenTypes.Slash;
    }
}