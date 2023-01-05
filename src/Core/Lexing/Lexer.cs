using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Core.Lexing.TokenParsers;
using Core.Utils.Diagnostic;

namespace Core.Lexing
{
    public class Lexer : ILexer
    {
        private readonly ITokenParser[] tokenParsers;

        public Lexer(ITokenParser[] tokenParsers)
        {
            this.tokenParsers = tokenParsers
                .OrderBy(parser => parser.Priority)
                .ToArray();
        }
        
        public TranslationState<ImmutableArray<SyntaxToken>> Tokenize(string text)
        {
            var diagnostic = new DiagnosticBag();
            var tokens = ImmutableArray.CreateBuilder<SyntaxToken>();
            
            foreach (var line in GetLines(text))
            {
                var position = 0;
                
                while (position < line.Length)
                {
                    var token = tokenParsers
                        .Select(parser => parser.TryParse(line, position, diagnostic))
                        .FirstOrDefault(token => token is not null);

                    token ??= ParseUnknownToken(line, position, diagnostic);
                    position += token.Location.Length;

                    tokens.Add(token);
                }
            }

            return new TranslationState<ImmutableArray<SyntaxToken>>(tokens.ToImmutable(), diagnostic);
        }

        private static SyntaxToken ParseUnknownToken(Line line, int position, DiagnosticBag diagnostic)
        {
            var terminal = line[position].ToString();
            var location = new Location(line, position, terminal.Length);
            var token = new SyntaxToken(TokenType.Unknown, terminal, location);
                        
            diagnostic.AddError(token.Location, $"Unknown token '{token.Text}'");

            return token;
        }
        
        private static IEnumerable<Line> GetLines(string text)
        {
            var lines = text.Split('\n');

            return lines
                .Select((line, i) =>
                {
                    var endLine = i == lines.Length - 1 ? '\0' : '\n';

                    return new Line(i, line + endLine);
                });
        }
    }
}