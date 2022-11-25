using Interpreter.Core.Execution;
using Interpreter.Core.Execution.Objects;
using Interpreter.Core.Lexing;

namespace Interpreter.Core.Syntax.AST.Expressions
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