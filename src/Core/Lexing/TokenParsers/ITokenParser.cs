using Core.Utils.Diagnostic;

namespace Core.Lexing.TokenParsers
{
    public interface ITokenParser
    {
        public Priority Priority { get; }

        SyntaxToken TryParse(Line line, int position, DiagnosticBag diagnostic);
    }
}