using Core.Execution;
using Core.Execution.Objects;
using Core.Lexing;

namespace Core.Syntax.AST.Expressions
{
    public class VariableExpression : Expression
    {
        public VariableExpression(SyntaxToken name)
        {
            Name = name;
        }
        
        public SyntaxToken Name { get; }

        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}