using Interpreter.Core.Lexing;

namespace Interpreter.Core.Execution.Operations.Binary
{
    public class DivisionOperation : BinaryOperation
    {
        protected override string OperatorMethodName => "op_Division";

        protected override TokenTypes Operator => TokenTypes.Slash;
    }
}