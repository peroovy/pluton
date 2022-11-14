namespace Translator.Core.Lexing.TokenParsers
{
    public interface ITokenParser
    {
        bool IsStartingFrom(string code, int position);

        SyntaxToken Parse(string code, int position);
    }
}