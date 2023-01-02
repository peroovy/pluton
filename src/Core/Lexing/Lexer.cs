using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Core.Diagnostic;
using Core.Lexing.TokenParsers;
using Core.Text;

namespace Core.Lexing
{
    public class Lexer : ILexer
    {
        private readonly ITokenParser[] tokenParsers;
        private readonly IDiagnosticBag diagnosticBag;

        public Lexer(ITokenParser[] tokenParsers, IDiagnosticBag diagnosticBag)
        {
            this.tokenParsers = tokenParsers
                .OrderBy(parser => parser.Priority)
                .ToArray();
            this.diagnosticBag = diagnosticBag;
        }

        public ImmutableArray<SyntaxToken> Tokenize(ImmutableArray<Line> lines)
        {
            return ParseTokens(lines)
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
                        .Select(parser => parser.TryParse(line, position))
                        .FirstOrDefault(token => token is not null);

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
                line[position].ToString(),
                new TextLocation(line, position)
            );
                        
            diagnosticBag.AddError(unknown.Location, unknown.Length, $"Unknown token: '{unknown.Text}'");

            return unknown;
        }
    }
}