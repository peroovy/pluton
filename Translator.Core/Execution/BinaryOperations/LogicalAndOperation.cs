using Translator.Core.Lexing;

namespace Translator.Core.Execution.BinaryOperations
{
    public class LogicalAndOperation : BinaryOperation
    {
        protected override string OperatorMethodName => "op_BitwiseAnd";

        protected override TokenTypes Operator => TokenTypes.DoubleAmpersand;
    }
}