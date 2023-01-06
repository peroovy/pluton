using System;
using Core.Utils;
using Core.Utils.Diagnostic;
using Core.Utils.Text;

namespace Core.Lexing.TokenParsers
{
    public class NextLineParser : ITokenParser
    {
        public Priority Priority => Priority.Low;
        
        public SyntaxToken TryParse(SourceText text, int position, DiagnosticBag diagnostic)
        {
            var newLine = string.Concat(text.Value.Take(position, 2));
            var location = new Location(text, position, newLine.Length);

            return newLine == Environment.NewLine
                ? new SyntaxToken(TokenType.LineBreak, newLine, location) 
                : null;
        }
    }
}