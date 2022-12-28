using System;
using NUnit.Framework;
using Interpreter.Core.Lexing;
using Interpreter.Core.Lexing.TokenParsers;
using Interpreter.Core.Lexing.TokenParsers.Words;
using Interpreter.Core.Text;
using Interpreter.Core.Logging;
using System.Collections.Immutable;

namespace Interpreter.Tests.LexingTests
{
    [TestFixture]
    public class LexerShould
    {
        private ILexer lexer;

        [SetUp]
        public void SetUp()
        {
            lexer = Core.InterpreterCore.Create().Lexer;
        }

        private void AssertTokenEqual(SyntaxToken expected, SyntaxToken actual)
        {
            
        }

        private void LexerTokenizeCorrectly(Line[] lines, SyntaxToken[] expected)
        {

            var actual = lexer.Tokenize(lines.ToImmutableArray());
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i].Type, actual[i].Type);
                Assert.AreEqual(expected[i].Text, actual[i].Text);
                Assert.AreEqual(expected[i].Location.Line.Number, actual[i].Location.Line.Number);
                Assert.AreEqual(expected[i].Location.Position, actual[i].Location.Position);
            }
        }

        [Test]
        public void LexerTokenizeCorrectlyAllTokenTypes()
        {
            var lines = new Line[]
            {
                new Line(0, "if (c == 3) {"),
                new Line(1, "   a = \"Success\";"),
                new Line(1, "}"),
                new Line(2, "else {"),
                new Line(3, "   a = \"oh no.."),
                new Line(3, "}\0"),
            };
            var tokens = new SyntaxToken[]
            {
                new SyntaxToken(TokenTypes.IfKeyword, "if", new TextLocation(lines[0], 0)),
                new SyntaxToken(TokenTypes.OpenParenthesis, "(", new TextLocation(lines[0], 3)),
                new SyntaxToken(TokenTypes.Identifier, "c", new TextLocation(lines[0], 4)),
                new SyntaxToken(TokenTypes.DoubleEquals, "==", new TextLocation(lines[0], 6)),
                new SyntaxToken(TokenTypes.Number, "3", new TextLocation(lines[0], 9)),
                new SyntaxToken(TokenTypes.CloseParenthesis, ")", new TextLocation(lines[0], 10)),
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation(lines[0], 12)),
                
                new SyntaxToken(TokenTypes.Identifier, "a", new TextLocation(lines[1], 3)),
                new SyntaxToken(TokenTypes.Equals, "=", new TextLocation(lines[1], 5)),
                new SyntaxToken(TokenTypes.String, "Success", new TextLocation(lines[1], 7)),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation(lines[1], 16)),
                
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation(lines[2], 0)),
                
                new SyntaxToken(TokenTypes.ElseKeyword, "else", new TextLocation(lines[3], 0)),
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation(lines[3], 5)),
                
                new SyntaxToken(TokenTypes.Identifier, "a", new TextLocation(lines[4], 3)),
                new SyntaxToken(TokenTypes.Equals, "=", new TextLocation(lines[4], 5)),
                new SyntaxToken(TokenTypes.Unknown, "\"", new TextLocation(lines[4], 7)),
                new SyntaxToken(TokenTypes.Identifier, "oh", new TextLocation(lines[4], 8)),
                new SyntaxToken(TokenTypes.Identifier, "no", new TextLocation(lines[4], 11)),
                new SyntaxToken(TokenTypes.Unknown, ".", new TextLocation(lines[4], 13)),
                new SyntaxToken(TokenTypes.Unknown, ".", new TextLocation(lines[4], 14)),
                
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation(lines[5], 0)),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation(lines[5], 1)),
            };
            
            LexerTokenizeCorrectly(lines, tokens);
        }
    }
}