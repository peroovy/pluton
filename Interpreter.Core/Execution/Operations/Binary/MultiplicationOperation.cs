using Interpreter.Core.Lexing;

namespace Interpreter.Core.Execution.Operations.Binary
{
    public class MultiplicationOperation : BinaryOperation
    {
        protected override string OperatorMethodName => "op_Multiply";

        protected override TokenTypes Operator => TokenTypes.Star;
    }
}