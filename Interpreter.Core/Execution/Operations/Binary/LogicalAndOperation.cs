using Interpreter.Core.Lexing;

namespace Interpreter.Core.Execution.Operations.Binary
{
    public class LogicalAndOperation : BinaryOperation
    {
        protected override string OperatorMethodName => "op_BitwiseAnd";

        protected override TokenTypes Operator => TokenTypes.AndKeyword;
    }
}