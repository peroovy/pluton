using Core.Lexing;
using Core.Syntax.AST.Expressions.Literals;

namespace Core.Syntax.AST
{
    public class InheritanceSyntax
    {
        public InheritanceSyntax(SyntaxToken colon, VariableExpression variable)
        {
            Colon = colon;
            Variable = variable;
        }
        
        public SyntaxToken Colon { get; }
        
        public VariableExpression Variable { get; }
    }
}