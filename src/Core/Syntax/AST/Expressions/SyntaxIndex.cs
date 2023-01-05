using Core.Lexing;

namespace Core.Syntax.AST.Expressions
{
    public class SyntaxIndex
    {
        public SyntaxIndex(SyntaxToken openBracket, Expression value, SyntaxToken closeBracket)
        {
            OpenBracket = openBracket;
            Value = value;
            CloseBracket = closeBracket;
        }  
        
        public SyntaxToken OpenBracket { get; }
        
        public Expression Value { get; }
        
        public SyntaxToken CloseBracket { get; }
    }
}