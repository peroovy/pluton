using Interpreter.Core.Lexing;
using Interpreter.Core.Execution.Operation.Binary;

namespace Interpreter.Core.Execution.Operations.Binary
{
    public class AdditiveOperation : BinaryOperation
    {
        protected override string OperatorMethodName => "op_Addition";

        protected override TokenTypes Operator => TokenTypes.Plus;
    }
}