using Translator.Core.Execution;
using Translator.Core.Lexing;

namespace Translator.Core.Syntax.AST.Expressions
{
    public class BinaryExpression : Expression
    {
        public BinaryExpression(Expression left, SyntaxToken operatorToken, Expression right)
        {
            Left = left;
            OperatorToken = operatorToken;
            Right = right;
        }
        
        public Expression Left { get; }
        
        public SyntaxToken OperatorToken { get; }
        
        public Expression Right { get; }

        public override Object Accept(IExecutor executor) => executor.Execute(this);
    }
}