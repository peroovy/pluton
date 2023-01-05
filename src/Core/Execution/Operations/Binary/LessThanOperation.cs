using Core.Lexing;

namespace Core.Execution.Operations.Binary
{
    public class LessThanOperation : BinaryOperation
    {
        public override TokenType Operator => TokenType.LeftArrow;

        public override OperationPrecedence Precedence => OperationPrecedence.Relational;
        
        protected override string MethodName => "op_LessThan";
    }
}