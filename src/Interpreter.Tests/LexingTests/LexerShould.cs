using System;
using System.Collections.Generic;
using NUnit.Framework;
using Core.Lexing;
using Core.Lexing.TokenParsers;
using System.Collections.Immutable;
using System.Linq;
using Core.Utils.Text;
using Ninject;

namespace Interpreter.Tests.LexingTests
{
    [TestFixture]
    public class LexerShould
    {
        private ILexer lexer;

        [SetUp]
        public void SetUp()
        {
            var container = Core.Interpreter.ConfigureContainer();
            lexer = container.Get<ILexer>();
        }

        private void LexerTokenizeCorrectly(SourceText lines, ImmutableArray<SyntaxToken> expected)
        {

            var actual = lexer.Tokenize(lines).Result;
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i].Type, actual[i].Type);
                Assert.AreEqual(expected[i].Text, actual[i].Text);
                Assert.AreEqual(expected[i].Location.Span.Start, actual[i].Location.Span.Start);
                Assert.AreEqual(expected[i].Location.Span.Length, actual[i].Location.Span.Length);
            }
        }
        
        private ImmutableArray<SyntaxToken> GetTokens(List<Tuple<TokenType, string>> values, SourceText stext)
        {
            var pos = 0;
            var result = new List<SyntaxToken>();
            foreach (var value in values)
            {
                var len = value.Item2.Length;
                if (value.Item1 == TokenType.String)
                    len += 2;
                result.Add(new SyntaxToken(value.Item1, value.Item2, new Location(stext, pos, len)));
                pos += len;
            }

            return result.ToImmutableArray();
        }

        [Test]
        public void LexerTokenizeCorrectlyAllTokenTypes()
        {
            var lines = new string[]
            {
                "if (c == 3) {",
                "   a = \"Success\";",
                "}",
                "else {",
                "   a = \"oh no..",
                "}\0",
            };
            var stext = new SourceText(string.Join("\n", lines));
            var t = new[]
            {
                Tuple.Create(TokenType.IfKeyword, "if"),
                Tuple.Create(TokenType.Space, " "),
                Tuple.Create(TokenType.OpenParenthesis, "("),
                Tuple.Create(TokenType.Identifier, "c"),
                Tuple.Create(TokenType.Space, " "),
                Tuple.Create(TokenType.DoubleEquals, "=="),
                Tuple.Create(TokenType.Space, " "),
                Tuple.Create(TokenType.Number, "3"),
                Tuple.Create(TokenType.CloseParenthesis, ")"),
                Tuple.Create(TokenType.Space, " "),
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.Space, "\n   "),
                
                Tuple.Create(TokenType.Identifier, "a"),
                Tuple.Create(TokenType.Space, " "),
                Tuple.Create(TokenType.Equals, "="),
                Tuple.Create(TokenType.Space, " "),
                Tuple.Create(TokenType.String, "Success"),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.Space, "\n"),
                
                Tuple.Create(TokenType.CloseBrace, "}"),
                Tuple.Create(TokenType.Space, "\n"),
                
                Tuple.Create(TokenType.ElseKeyword, "else"),
                Tuple.Create(TokenType.Space, " "),
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.Space, "\n   "),
                
                Tuple.Create(TokenType.Identifier, "a"),
                Tuple.Create(TokenType.Space, " "),
                Tuple.Create(TokenType.Equals, "="),
                Tuple.Create(TokenType.Space, " "),
                Tuple.Create(TokenType.Unknown, "\""),
                Tuple.Create(TokenType.Identifier, "oh"),
                Tuple.Create(TokenType.Space, " "),
                Tuple.Create(TokenType.Identifier, "no"),
                Tuple.Create(TokenType.Unknown, "."),
                Tuple.Create(TokenType.Unknown, "."),
                Tuple.Create(TokenType.Space, "\n"),
                
                Tuple.Create(TokenType.CloseBrace, "}"),
                Tuple.Create(TokenType.Eof, "\0"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();

            var tokens = GetTokens(t, stext);
            
            LexerTokenizeCorrectly(stext, tokens);
        }
    }
}