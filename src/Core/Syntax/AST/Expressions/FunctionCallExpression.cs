using System.Collections.Immutable;
using Core.Execution;
using Core.Execution.Objects;
using Core.Lexing;

namespace Core.Syntax.AST.Expressions
{
    public class FunctionCallExpression : Expression
    {
        public FunctionCallExpression(SyntaxToken name, SyntaxToken openParenthesis, ImmutableArray<Expression> arguments, SyntaxToken closeParenthesis)
        {
            Name = name;
            OpenParenthesis = openParenthesis;
            Arguments = arguments;
            CloseParenthesis = closeParenthesis;
        }
        
        public SyntaxToken Name { get; }
        
        public SyntaxToken OpenParenthesis { get; }
        
        public ImmutableArray<Expression> Arguments { get; }
        
        public SyntaxToken CloseParenthesis { get; }
        
        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}