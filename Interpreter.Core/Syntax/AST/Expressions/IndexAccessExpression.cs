using System.Collections.Immutable;
using Interpreter.Core.Execution;
using Interpreter.Core.Execution.Objects;
using Interpreter.Core.Lexing;

namespace Interpreter.Core.Syntax.AST.Expressions
{
    public class IndexAccessExpression : Expression
    {
        public IndexAccessExpression(Expression expression, SyntaxToken openBracket, Expression index, SyntaxToken closeBracket)
        {
            Expression = expression;
            OpenBracket = openBracket;
            Index = index;
            CloseBracket = closeBracket;
        }
        
        public Expression Expression { get; }
        
        public SyntaxToken OpenBracket { get; }
        
        public Expression Index { get; }
        
        public SyntaxToken CloseBracket { get; }
        
        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}