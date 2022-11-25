using Interpreter.Core.Lexing;

namespace Interpreter.Core.Execution.Operations.Unary
{
    public class UnaryPlusOperation : UnaryOperation
    {
        protected override string OperatorMethodName => "op_UnaryPlus";

        protected override TokenTypes Operator => TokenTypes.Plus;
    }
}