using Interpreter.Core.Logging;
using Interpreter.Core.Text;

namespace Interpreter.Core.Lexing.TokenParsers
{
    public class WhitespaceParser : ITokenParser
    {
        public int Priority => 3;
        
        public bool CanParseFrom(Line line, int position) => char.IsWhiteSpace(line.Value[position]);

        public SyntaxToken Parse(Line line, int position, ILogger logger)
        {
            var location = new TextLocation(line, position);
            
            if (line.Value[position] == '\n')
                return new SyntaxToken(TokenTypes.LineSeparator,"\n", location);

            var value = string.Concat(
                line.Value.TakeWhileFrom(sym => char.IsWhiteSpace(sym) && sym != '\n', position)
            );

            return new SyntaxToken(TokenTypes.Space, value, location);
        }
    }
}