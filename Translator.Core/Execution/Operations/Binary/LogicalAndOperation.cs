using Translator.Core.Execution.Operations.Binary;
using Translator.Core.Lexing;

namespace Translator.Core.Execution.Operation.Binary
{
    public class LogicalAndOperation : BinaryOperation
    {
        protected override string OperatorMethodName => "op_BitwiseAnd";

        protected override TokenTypes Operator => TokenTypes.DoubleAmpersand;
    }
}