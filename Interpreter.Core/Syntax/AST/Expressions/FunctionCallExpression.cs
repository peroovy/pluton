using System.Collections.Immutable;
using Interpreter.Core.Execution;
using Interpreter.Core.Execution.Objects;
using Interpreter.Core.Lexing;

namespace Interpreter.Core.Syntax.AST.Expressions
{
    public class FunctionCallExpression : Expression
    {
        public FunctionCallExpression(SyntaxToken name, SyntaxToken openParenthesis, ImmutableArray<Expression> positionArguments, SyntaxToken closeParenthesis)
        {
            Name = name;
            OpenParenthesis = openParenthesis;
            PositionArguments = positionArguments;
            CloseParenthesis = closeParenthesis;
        }
        
        public SyntaxToken Name { get; }
        
        public SyntaxToken OpenParenthesis { get; }
        
        public ImmutableArray<Expression> PositionArguments { get; }
        
        public SyntaxToken CloseParenthesis { get; }
        
        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}