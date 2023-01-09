using System.Collections.Generic;
using Core.Execution;
using Core.Execution.Objects;
using Core.Lexing;
using Core.Utils.Text;

namespace Core.Syntax.AST.Expressions
{
    public class BinaryExpression : Expression
    {
        public BinaryExpression(SourceText sourceText, Expression left, SyntaxToken operatorToken, Expression right)
            : base(sourceText)
        {
            Left = left;
            OperatorToken = operatorToken;
            Right = right;
        }
        
        public Expression Left { get; }
        
        public SyntaxToken OperatorToken { get; }
        
        public Expression Right { get; }

        public override SyntaxToken FirstChild => Left.FirstChild;

        public override SyntaxToken LastChild => Right.LastChild;
        
        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}