using System.Collections.Generic;
using System.Linq;
using Translator.Core.Lexing.TokenParsers;

namespace Translator.Core.Lexing
{
    public class Lexer
    {
        private readonly ITokenParser[] tokenParsers;

        public Lexer(ITokenParser[] tokenParsers)
        {
            this.tokenParsers = tokenParsers;
        }

        public IReadOnlyList<SyntaxToken> Tokenize(string code)
        {
            return ParseTokens(code)
                .Where(token => token.Type != TokenTypes.Space && token.Type != TokenTypes.LineSeparator)
                .ToList()
                .AsReadOnly();
        }

        private IEnumerable<SyntaxToken> ParseTokens(string code)
        {
            var position = 0;

            while (position < code.Length)
            {
                var token = tokenParsers
                    .FirstOrDefault(p => p.IsStartingFrom(code, position))
                    ?.Parse(code, position);

                token ??= new SyntaxToken(TokenTypes.Unknown, code[position].ToString());
                position += token.Lenght;
                
                yield return token;
            }
        }
    }
}