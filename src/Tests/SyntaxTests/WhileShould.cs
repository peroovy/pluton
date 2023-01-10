using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Core.Lexing;
using Core.Syntax;
using Core.Syntax.AST;
using Core.Syntax.AST.Expressions;
using Core.Utils.Text;
using Ninject;
using NUnit.Framework;

namespace Tests.SyntaxTests
{
    [TestFixture]
    public class WhileShould
    {
        private ISyntaxParser parser;
        
        [SetUp]
        public void SetUp()
        {
            parser = Core.Interpreter.ConfigureContainer().Get<ISyntaxParser>();
        }

        private WhileStatement WhileSetUp(ImmutableArray<SyntaxToken> tokens)
        {
            var tree = parser.Parse(tokens.Length == 0 ? new SourceText("") : tokens[0].Location.SourceText, 
                tokens);
            Assert.IsFalse(tree.HasErrors);
            Assert.AreEqual(1, tree.Result.Members.Length);
            Assert.IsTrue(tree.Result.Members[0] is WhileStatement);
            return (WhileStatement)tree.Result.Members[0];
        }
        
        private ImmutableArray<SyntaxToken> GetTokens(List<Tuple<TokenType, string>> values)
        {
            var text = string.Join("", values.Select(e => e.Item2));
            var stext = new SourceText(text);
            var pos = 0;
            var result = new List<SyntaxToken>();
            foreach (var value in values)
            {
                var len = value.Item2.Length;
                result.Add(new SyntaxToken(value.Item1, value.Item2, new Location(stext, pos, len)));
                pos += len;
            }

            return result.ToImmutableArray();
        }

        [Test]
        public void BlockEmptyBody()
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.WhileKeyword, "while"),
                Tuple.Create(TokenType.OpenParenthesis, "("),
                Tuple.Create(TokenType.TrueKeyword, "true"),
                Tuple.Create(TokenType.CloseParenthesis, ")"),
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var w = WhileSetUp(GetTokens(tokens));
            Assert.IsTrue(w.Body is BlockStatement);
            Assert.AreEqual(0, ((BlockStatement)w.Body).Statements.Length);
        }

        [Test]
        public void BlockBody()
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.WhileKeyword, "while"),
                Tuple.Create(TokenType.OpenParenthesis, "("),
                Tuple.Create(TokenType.TrueKeyword, "true"),
                Tuple.Create(TokenType.CloseParenthesis, ")"),
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.BreakKeyword, "break"),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var w = WhileSetUp(GetTokens(tokens));
            Assert.IsTrue(w.Body is BlockStatement);
            Assert.AreEqual(1, ((BlockStatement)w.Body).Statements.Length);
        }

        [Test]
        public void NotBlockBody()
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.WhileKeyword, "while"),
                Tuple.Create(TokenType.OpenParenthesis, "("),
                Tuple.Create(TokenType.TrueKeyword, "true"),
                Tuple.Create(TokenType.CloseParenthesis, ")"),
                Tuple.Create(TokenType.BreakKeyword, "break"),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var w = WhileSetUp(GetTokens(tokens));
            Assert.IsTrue(w.Body is BreakStatement);
        }

        [Test]
        public void BoolCondition()
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.WhileKeyword, "while"),
                Tuple.Create(TokenType.OpenParenthesis, "("),
                Tuple.Create(TokenType.TrueKeyword, "true"),
                Tuple.Create(TokenType.CloseParenthesis, ")"),
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var w = WhileSetUp(GetTokens(tokens));
            Assert.IsTrue(w.Condition is BooleanExpression);
        }

        [Test]
        public void BinCondition()
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.WhileKeyword, "while"),
                Tuple.Create(TokenType.OpenParenthesis, "("),
                Tuple.Create(TokenType.Identifier, "a"),
                Tuple.Create(TokenType.LeftArrow, "<"),
                Tuple.Create(TokenType.Number, "5"),
                Tuple.Create(TokenType.CloseParenthesis, ")"),
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var w = WhileSetUp(GetTokens(tokens));
            Assert.IsTrue(w.Condition is BinaryExpression);
        }
    }
}