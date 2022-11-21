using Translator.Core.Execution;

namespace Translator.Core.Syntax.AST.Expressions
{
    public class BooleanExpression : Expression
    {
        public BooleanExpression(bool value)
        {
            Value = value;
        }
        
        public bool Value { get; }
        
        public override Object Accept(IExecutor executor) => executor.Execute(this);
    }
}