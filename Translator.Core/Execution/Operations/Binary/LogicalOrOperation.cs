using Translator.Core.Lexing;

namespace Translator.Core.Execution.Operations.Binary
{
    public class LogicalOrOperation : BinaryOperation
    {
        protected override string OperatorMethodName => "op_BitwiseOr";

        protected override TokenTypes Operator => TokenTypes.DoublePipe;
    }
}