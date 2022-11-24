using Translator.Core.Execution;
using Translator.Core.Execution.Objects;
using Translator.Core.Lexing;
using Translator.Core.Syntax.AST.Expressions;

namespace Translator.Core.Syntax.AST
{
    public class Condition : SyntaxNode
    {
        public Condition(SyntaxToken openParenthesis, Expression expression, SyntaxToken closeParenthesis)
        {
            OpenParenthesis = openParenthesis;
            Expression = expression;
            CloseParenthesis = closeParenthesis;
        }
        
        public SyntaxToken OpenParenthesis { get; }
        
        public Expression Expression { get; }
        
        public SyntaxToken CloseParenthesis { get; }

        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}