using Translator.Core.Lexing;

namespace Translator.Core.Execution.BinaryOperations
{
    public class AdditiveOperation : BinaryOperation
    {
        protected override string OperatorMethodName => "op_Addition";

        protected override TokenTypes Operator => TokenTypes.Plus;
    }
}