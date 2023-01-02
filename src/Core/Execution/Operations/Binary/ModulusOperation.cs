using Core.Lexing;

namespace Core.Execution.Operations.Binary
{
    public class ModulusOperation : BinaryOperation
    {
        protected override string OperatorMethodName => "op_Modulus";

        protected override TokenTypes Operator => TokenTypes.Percent;
    }
}