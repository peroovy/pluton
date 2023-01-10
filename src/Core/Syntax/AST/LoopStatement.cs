using Core.Syntax.AST.Expressions;

namespace Core.Syntax.AST
{
    public abstract class LoopStatement : Statement
    {
        public abstract Expression Condition { get; }
        
        public abstract Statement Body { get; }
    }
}