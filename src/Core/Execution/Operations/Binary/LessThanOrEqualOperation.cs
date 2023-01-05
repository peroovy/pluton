using Core.Lexing;

namespace Core.Execution.Operations.Binary
{
    public class LessThanOrEqualOperation : BinaryOperation
    {
        public override TokenType Operator => TokenType.LeftArrowEquals;

        public override OperationPrecedence Precedence => OperationPrecedence.Relational;
        
        protected override string MethodName => "op_LessThanOrEqual";
    }
}