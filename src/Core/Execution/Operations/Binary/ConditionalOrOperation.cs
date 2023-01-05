using Core.Lexing;

namespace Core.Execution.Operations.Binary
{
    public class ConditionalOrOperation : BinaryOperation
    {
        public override TokenType Operator => TokenType.OrKeyword;

        public override TokenType? CompoundAssignmentOperator => null;

        public override OperationPrecedence Precedence => OperationPrecedence.ConditionalOr;
        
        protected override string MethodName => "op_BitwiseOr";
    }
}