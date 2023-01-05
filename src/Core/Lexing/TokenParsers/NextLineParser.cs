using System;
using Core.Utils;
using Core.Utils.Text;

namespace Core.Lexing.TokenParsers
{
    public class NextLineParser : ITokenParser
    {
        public int Priority => 0;
        
        public SyntaxToken TryParse(Line line, int position)
        {
            var newLine = string.Concat(line.Value.Take(position, 2));
            var location = new Location(line, position, newLine.Length);

            return newLine == Environment.NewLine
                ? new SyntaxToken(TokenTypes.NewLine, newLine, location) 
                : null;
        }
    }
}