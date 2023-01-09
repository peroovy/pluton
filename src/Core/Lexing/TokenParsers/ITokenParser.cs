using Core.Utils.Diagnostic;
using Core.Utils.Text;

namespace Core.Lexing.TokenParsers
{
    public interface ITokenParser
    {
        public Priority Priority { get; }

        SyntaxToken TryParse(SourceText text, int position, DiagnosticBag diagnostic);
    }
}