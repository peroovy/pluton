using Translator.Core.Execution.Objects;
using Translator.Core.Lexing;

namespace Translator.Core.Execution.BinaryOperations
{
    public class SubtractionOperation : BinaryOperation
    {
        protected override string OperatorMethodName => "op_Subtraction";

        protected override TokenTypes Operator => TokenTypes.Minus;
    }
}