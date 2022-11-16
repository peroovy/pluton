using System.Linq;
using Translator.Core.Syntax.AST;

namespace Translator.Core.Evaluation
{
    public class Evaluator : IVisitor<object>
    {
        private readonly IBinaryOperation[] binaryOperations;

        public Evaluator(IBinaryOperation[] binaryOperations)
        {
            this.binaryOperations = binaryOperations;
        }

        public object Visit(ParenthesizedExpression expression) => expression.InnerExpression.Accept(this);

        public object Visit(BinaryExpression binary)
        {
            var left = binary.Left.Accept(this);
            var right = binary.Right.Accept(this);
            var op = binaryOperations.First(op =>
                op.CanEvaluateForOperands(left, binary.OperatorToken.Type, right)
            );

            return op.Evaluate(left, right);
        }

        public object Visit(NumberExpression number) => number.Value;
    }
}