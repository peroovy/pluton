using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Core.Lexing.TokenParsers;
using Core.Utils.Diagnostic;
using Core.Utils.Text;

namespace Core.Lexing
{
    public class Lexer : ILexer
    {
        private readonly ImmutableArray<ITokenParser> tokenParsers;

        public Lexer(IEnumerable<ITokenParser> tokenParsers)
        {
            this.tokenParsers = tokenParsers
                .OrderByDescending(parser => parser.Priority)
                .ToImmutableArray();
        }
        
        public TranslationState<ImmutableArray<SyntaxToken>> Tokenize(SourceText sourceText)
        {
            var tokens = ImmutableArray.CreateBuilder<SyntaxToken>();
            var diagnosticBag = new DiagnosticBag();

            var position = 0;
            while (position < sourceText.Length)
            {
                var token = tokenParsers
                    .Select(parser => parser.TryParse(sourceText, position, diagnosticBag))
                    .FirstOrDefault(token => token is not null);

                token ??= ParseUnknownToken(sourceText, position, diagnosticBag);
                position += token.Location.Span.Length;

                tokens.Add(token);
            }

            return new TranslationState<ImmutableArray<SyntaxToken>>(tokens.ToImmutable(), diagnosticBag);
        }

        private static SyntaxToken ParseUnknownToken(SourceText text, int position, DiagnosticBag diagnostic)
        {
            var character = text[position].ToString();
            var location = new Location(text, position, character.Length);
            var token = new SyntaxToken(TokenType.Unknown, character, location);
                        
            diagnostic.AddError(token.Location, $"Unknown token '{token.Text}'");

            return token;
        }
    }
}