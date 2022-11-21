using Translator.Core.Execution;
using Translator.Core.Execution.Objects;
using Translator.Core.Lexing;

namespace Translator.Core.Syntax.AST.Expressions
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

        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}