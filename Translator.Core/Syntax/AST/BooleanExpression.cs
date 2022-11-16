using Translator.Core.Evaluation;

namespace Translator.Core.Syntax.AST
{
    public class BooleanExpression : Expression
    {
        public BooleanExpression(bool value)
        {
            Value = value;
        }
        
        public bool Value { get; }
        
        public override object Accept(IEvaluator evaluator) => evaluator.Evaluate(this);
    }
}