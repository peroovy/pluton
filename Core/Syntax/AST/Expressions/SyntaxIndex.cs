using Core.Lexing;

namespace Core.Syntax.AST.Expressions
{
    public class SyntaxIndex
    {
        public SyntaxIndex(SyntaxToken openBracket, Expression index, SyntaxToken closeBracket)
        {
            OpenBracket = openBracket;
            Index = index;
            CloseBracket = closeBracket;
        }  
        
        public SyntaxToken OpenBracket { get; }
        
        public Expression Index { get; }
        
        public SyntaxToken CloseBracket { get; }
    }
}