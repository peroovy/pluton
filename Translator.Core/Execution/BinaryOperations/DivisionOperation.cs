using Translator.Core.Execution.Objects;
using Translator.Core.Lexing;

namespace Translator.Core.Execution.BinaryOperations
{
    public class DivisionOperation : BinaryOperation
    {
        protected override string OperatorMethodName => "op_Division";

        protected override TokenTypes Operator => TokenTypes.Slash;
    }
}