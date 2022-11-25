using Interpreter.Core.Text;

namespace Interpreter.Core.Lexing.TokenParsers
{
    public interface ITokenParser
    {
        public int Priority { get; }
        
        bool CanParseFrom(Line line, int position);

        SyntaxToken Parse(Line line, int position);
    }
}