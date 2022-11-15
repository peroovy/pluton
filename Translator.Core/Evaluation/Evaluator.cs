using System.Linq;
using Translator.Core.Syntax.AST;

namespace Translator.Core.Evaluation
{
    public class Evaluator : IVisitor<object>
    {
        private readonly IBinaryOperation[] operations;

        public Evaluator(IBinaryOperation[] operations)
        {
            this.operations = operations;
        }

        public object Visit(ParenthesizedExpression expression) => expression.InnerExpression.Accept(this);

        public object Visit(BinaryExpression binary)
        {
            var left = binary.Left.Accept(this);
            var right = binary.Right.Accept(this);
            var op = operations.First(op =>
                op.CanEvaluateForOperands(left, binary.OperatorToken.Type, right)
            );

            return op.Evaluate(left, right);
        }

        public object Visit(NumberExpression number) => number.Value;
    }
}