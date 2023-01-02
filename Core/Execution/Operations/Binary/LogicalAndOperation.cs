using Core.Lexing;

namespace Core.Execution.Operations.Binary
{
    public class LogicalAndOperation : BinaryOperation
    {
        protected override string OperatorMethodName => "op_BitwiseAnd";

        protected override TokenTypes Operator => TokenTypes.AndKeyword;
    }
}