using System.Collections.Generic;
using Core.Execution;
using Core.Execution.Objects;
using Core.Lexing;
using Core.Utils.Text;

namespace Core.Syntax.AST.Expressions
{
    public class UnaryExpression : Expression
    {
        public UnaryExpression(SourceText sourceText, SyntaxToken operatorToken, Expression operand)
            : base(sourceText)
        {
            OperatorToken = operatorToken;
            Operand = operand;
        }
        
        public SyntaxToken OperatorToken { get; }
        
        public Expression Operand { get; }
        
        public override Obj Accept(IExecutor executor) => executor.Execute(this);
        
        public override IEnumerable<Location> GetChildrenLocations()
        {
            yield return OperatorToken.Location;
            yield return Operand.Location;
        }
    }
}