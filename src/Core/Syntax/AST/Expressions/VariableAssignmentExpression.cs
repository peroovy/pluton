using System.Collections.Generic;
using Core.Execution;
using Core.Execution.Objects;
using Core.Lexing;
using Core.Utils.Text;

namespace Core.Syntax.AST.Expressions
{
    public class VariableAssignmentExpression : Expression
    {
        public VariableAssignmentExpression(
            SourceText sourceText, SyntaxToken identifier, SyntaxToken equalsToken, Expression expression)
            : base(sourceText)
        {
            Identifier = identifier;
            EqualsToken = equalsToken;
            Expression = expression;
        }
        
        public SyntaxToken Identifier { get; }
        
        public SyntaxToken EqualsToken { get; }
        
        public Expression Expression { get; }

        public override SyntaxToken FirstChild => Identifier;

        public override SyntaxToken LastChild => Expression.LastChild;
        
        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}