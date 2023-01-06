using System.Collections.Generic;
using System.Collections.Immutable;
using Core.Execution;
using Core.Execution.Objects;
using Core.Lexing;
using Core.Utils.Text;

namespace Core.Syntax.AST.Expressions
{
    public class CallExpression : Expression
    {
        public CallExpression(
            SourceText sourceText,
            Expression callableExpression, 
            SyntaxToken openParenthesis, 
            ImmutableArray<Expression> arguments, 
            SyntaxToken closeParenthesis) : base(sourceText)
        {
            CallableExpression = callableExpression;
            OpenParenthesis = openParenthesis;
            Arguments = arguments;
            CloseParenthesis = closeParenthesis;
        }
        
        public Expression CallableExpression { get; }
        
        public SyntaxToken OpenParenthesis { get; }
        
        public ImmutableArray<Expression> Arguments { get; }
        
        public SyntaxToken CloseParenthesis { get; }

        public override SyntaxToken FirstChild => CallableExpression.FirstChild;

        public override SyntaxToken LastChild => CloseParenthesis;
        
        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}