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
            var tokens = ParseTokens(lines)
                .ToImmutableArray();

            return tokens;
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
                    position += token.Location.Length;

                    yield return token;
                }
            }
        }

        private SyntaxToken ParseUnknownToken(Line line, int position)
        {
            var terminal = line[position].ToString();
            var location = new Location(line, position, terminal.Length);
            var token = new SyntaxToken(TokenTypes.Unknown, terminal, location);
                        
            diagnosticBag.AddError(token.Location, $"Unknown token: '{token.Text}'");

            return token;
        }
    }
}