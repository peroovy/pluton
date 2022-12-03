using Interpreter.Core.Execution;
using Interpreter.Core.Execution.Objects;

namespace Interpreter.Core.Syntax.AST.Expressions
{
    public class IndexAccessExpression : Expression
    {
        public IndexAccessExpression(Expression parentExpression, SyntaxIndex index)
        {
            ParentExpression = parentExpression;
            Index = index;
        }
        
        public Expression ParentExpression { get; }
        
        public SyntaxIndex Index { get; }
        
        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}