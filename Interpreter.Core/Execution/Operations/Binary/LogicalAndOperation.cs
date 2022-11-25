using Interpreter.Core.Execution.Operations.Binary;
using Interpreter.Core.Lexing;

namespace Interpreter.Core.Execution.Operation.Binary
{
    public class LogicalAndOperation : BinaryOperation
    {
        protected override string OperatorMethodName => "op_BitwiseAnd";

        protected override TokenTypes Operator => TokenTypes.DoubleAmpersand;
    }
}