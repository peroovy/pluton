using Core.Lexing;

namespace Core.Execution.Operations.Binary
{
    public class AdditionOperation : BinaryOperation
    {
        public override TokenType Operator => TokenType.Plus;

        public override OperationPrecedence Precedence => OperationPrecedence.Additive;
        
        protected override string MethodName => "op_Addition";
    }
}