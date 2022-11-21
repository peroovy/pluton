using Translator.Core.Execution;
using Translator.Core.Lexing;

namespace Translator.Core.Syntax.AST.Expressions
{
    public class VariableExpression : Expression
    {
        public VariableExpression(SyntaxToken name)
        {
            Name = name;
        }
        
        public SyntaxToken Name { get; }

        public override Object Accept(IExecutor executor) => executor.Execute(this);
    }
}