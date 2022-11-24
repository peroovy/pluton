using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Translator.Core.Lexing.TokenParsers;
using Translator.Core.Logging;
using Translator.Core.Text;

namespace Translator.Core.Lexing
{
    public class Lexer : ILexer
    {
        private readonly ITokenParser[] tokenParsers;
        private readonly ILogger logger;

        public Lexer(ITokenParser[] tokenParsers, ILogger logger)
        {
            this.tokenParsers = tokenParsers;
            this.logger = logger;
        }

        public ImmutableArray<SyntaxToken> Tokenize(ImmutableArray<Line> lines)
        {
            return ParseTokens(lines)
                .Where(token => token.Type != TokenTypes.Space)
                .ToImmutableArray();
        }

        private IEnumerable<SyntaxToken> ParseTokens(ImmutableArray<Line> lines)
        {
            foreach (var line in lines)
            {
                var position = 0;
                
                while (position < line.Length)
                {
                    var token = tokenParsers
                        .SingleOrDefault(p => p.CanParseFrom(line, position))
                        ?.Parse(line, position);

                    token ??= ParseUnknownToken(line, position);
                    position += token.Length;

                    yield return token;
                }
            }
        }

        private SyntaxToken ParseUnknownToken(Line line, int position)
        {
            var unknown = new SyntaxToken(
                TokenTypes.Unknown,
                line.Value[position].ToString(),
                new TextLocation(line, position)
            );
                        
            logger.Error(unknown.Location, unknown.Length, $"Unknown token: '{unknown.Text}'");

            return unknown;
        }
    }
}