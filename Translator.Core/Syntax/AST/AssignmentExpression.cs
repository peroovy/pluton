using Translator.Core.Execution;
using Translator.Core.Lexing;

namespace Translator.Core.Syntax.AST
{
    public class AssignmentExpression : Expression
    {
        public AssignmentExpression(SyntaxToken variable, SyntaxToken equals, Expression expression)
        {
            Variable = variable;
            EqualsOperator = equals;
            Expression = expression;
        }
        
        public SyntaxToken Variable { get; }
        
        public SyntaxToken EqualsOperator { get; }
        
        public Expression Expression { get; }

        public override Object Accept(IExecutor executor) => executor.Execute(this);
    }
}