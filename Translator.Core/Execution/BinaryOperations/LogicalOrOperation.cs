using Translator.Core.Lexing;

namespace Translator.Core.Execution.BinaryOperations
{
    public class LogicalOrOperation : BinaryOperation
    {
        protected override string OperatorMethodName => "op_BitwiseOr";

        protected override TokenTypes Operator => TokenTypes.DoublePipe;
    }
}