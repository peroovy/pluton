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

        public IEnumerable<SyntaxToken> Tokenize(string code)
        {
            var tokens = new List<SyntaxToken>();
            
            foreach (var token in ParseTokens(code))
            {
                tokens.Add(token);
            }

            return tokens;
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