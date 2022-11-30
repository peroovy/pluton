using Interpreter.Core.Execution;
using Interpreter.Core.Execution.Objects;
using Interpreter.Core.Lexing;

namespace Interpreter.Core.Syntax.AST.Expressions
{
    public class CollectionIndexExpression : Expression
    {
        public CollectionIndexExpression(VariableExpression variable, SyntaxToken openBracket, Expression index, SyntaxToken closeBracket)
        {
            Variable = variable;
            OpenBracket = openBracket;
            Index = index;
            CloseBracket = closeBracket;
        }
        
        public VariableExpression Variable { get; }
        
        public SyntaxToken OpenBracket { get; }
        
        public Expression Index { get; }
        
        public SyntaxToken CloseBracket { get; }
        
        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}