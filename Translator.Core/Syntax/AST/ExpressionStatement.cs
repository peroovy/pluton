using Translator.Core.Execution;
using Translator.Core.Lexing;
using Translator.Core.Syntax.AST.Expressions;

namespace Translator.Core.Syntax.AST
{
    public class ExpressionStatement : Statement
    {

        public ExpressionStatement(Expression expression, SyntaxToken closingToken)
        {
            Expression = expression;
            ClosingToken = closingToken;
        }
        
        public Expression Expression { get; }
        
        public SyntaxToken ClosingToken { get; }

        public override Object Accept(IExecutor executor) => executor.Execute(this);
    }
}