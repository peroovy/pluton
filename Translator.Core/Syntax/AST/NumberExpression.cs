using Translator.Core.Execution;

namespace Translator.Core.Syntax.AST
{
    public class NumberExpression : Expression
    {
        public NumberExpression(double value)
        {
            Value = value;
        }
        
        public double Value { get; }

        public override object Accept(IExecutor executor) => executor.Execute(this);
    }
}