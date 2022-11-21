using Translator.Core.Execution;

namespace Translator.Core.Syntax.AST.Expressions
{
    public class NumberExpression : Expression
    {
        public NumberExpression(double value)
        {
            Value = value;
        }
        
        public double Value { get; }

        public override Object Accept(IExecutor executor) => executor.Execute(this);
    }
}