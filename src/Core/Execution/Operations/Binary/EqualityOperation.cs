using Core.Lexing;

namespace Core.Execution.Operations.Binary
{
    public class EqualityOperation : BinaryOperation
    {
        protected override string OperatorMethodName => "op_Equality";

        protected override TokenTypes Operator => TokenTypes.DoubleEquals;
    }
}