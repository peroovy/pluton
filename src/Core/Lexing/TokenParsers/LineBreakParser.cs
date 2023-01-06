using System;
using Core.Utils;
using Core.Utils.Diagnostic;
using Core.Utils.Text;

namespace Core.Lexing.TokenParsers
{
    public class LineBreakParser : ITokenParser
    {
        public Priority Priority => Priority.Low;
        
        public SyntaxToken TryParse(SourceText text, int position, DiagnosticBag diagnostic)
        {
            var lineBreak = string.Concat(text.Value.Take(position, 2));
            var location = new Location(text, position, lineBreak.Length);

            return lineBreak == Environment.NewLine
                ? new SyntaxToken(TokenType.LineBreak, lineBreak, location) 
                : null;
        }
    }
}