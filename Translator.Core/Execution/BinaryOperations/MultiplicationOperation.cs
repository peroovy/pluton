using Translator.Core.Execution.Objects;
using Translator.Core.Lexing;

namespace Translator.Core.Execution.BinaryOperations
{
    public class MultiplicationOperation : BinaryOperation
    {
        protected override string OperatorMethodName => "op_Multiply";

        protected override TokenTypes Operator => TokenTypes.Star;
    }
}