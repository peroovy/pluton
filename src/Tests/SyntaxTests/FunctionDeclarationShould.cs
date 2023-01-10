using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Core.Lexing;
using Core.Syntax;
using Core.Syntax.AST;
using Core.Utils.Text;
using Ninject;
using NUnit.Framework;

namespace Tests.SyntaxTests
{
    [TestFixture]
    public class FunctionDeclarationShould
    {
        private ISyntaxParser parser;
        
        [SetUp]
        public void SetUp()
        {
            parser = Core.Interpreter.ConfigureContainer().Get<ISyntaxParser>();
        }

        private FunctionDeclarationStatement FuncSetUp(ImmutableArray<SyntaxToken> tokens)
        {
            var tree = parser.Parse(tokens.Length == 0 ? new SourceText("") : tokens[0].Location.SourceText, 
                tokens);
            Assert.IsFalse(tree.HasErrors);
            Assert.AreEqual(1, tree.Result.Members.Length);
            Assert.IsTrue(tree.Result.Members[0] is FunctionDeclarationStatement);
            return (FunctionDeclarationStatement)tree.Result.Members[0];
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
        public void SimpleFunction()
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.DefKeyword, "def"),
                Tuple.Create(TokenType.Identifier, "a"),
                Tuple.Create(TokenType.OpenParenthesis, "("),
                Tuple.Create(TokenType.CloseParenthesis, ")"),
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var f = FuncSetUp(GetTokens(tokens));
        }

        [Test]
        public void NoParameters()
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.DefKeyword, "def"),
                Tuple.Create(TokenType.Identifier, "a"),
                Tuple.Create(TokenType.OpenParenthesis, "("),
                Tuple.Create(TokenType.CloseParenthesis, ")"),
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var f = FuncSetUp(GetTokens(tokens));
            Assert.AreEqual(0, f.PositionParameters.Length);
            Assert.AreEqual(0, f.DefaultParameters.Length);
        }

        [Test]
        public void SingleParameter()
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.DefKeyword, "def"),
                Tuple.Create(TokenType.Identifier, "a"),
                Tuple.Create(TokenType.OpenParenthesis, "("),
                Tuple.Create(TokenType.Identifier, "b"),
                Tuple.Create(TokenType.CloseParenthesis, ")"),
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var f = FuncSetUp(GetTokens(tokens));
            Assert.AreEqual(1, f.PositionParameters.Length);
            Assert.AreEqual(0, f.DefaultParameters.Length);
        }

        [Test]
        public void MultipleParameters()
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.DefKeyword, "def"),
                Tuple.Create(TokenType.Identifier, "a"),
                Tuple.Create(TokenType.OpenParenthesis, "("),
                Tuple.Create(TokenType.Identifier, "b"),
                Tuple.Create(TokenType.Comma, ","),
                Tuple.Create(TokenType.Identifier, "c"),
                Tuple.Create(TokenType.CloseParenthesis, ")"),
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var f = FuncSetUp(GetTokens(tokens));
            Assert.AreEqual(2, f.PositionParameters.Length);
            Assert.AreEqual(0, f.DefaultParameters.Length);
        }

        [Test]
        public void DefaultParameter()
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.DefKeyword, "def"),
                Tuple.Create(TokenType.Identifier, "a"),
                Tuple.Create(TokenType.OpenParenthesis, "("),
                Tuple.Create(TokenType.Identifier, "b"),
                Tuple.Create(TokenType.Equals, "="),
                Tuple.Create(TokenType.Number, "5"),
                Tuple.Create(TokenType.CloseParenthesis, ")"),
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var f = FuncSetUp(GetTokens(tokens));
            Assert.AreEqual(0, f.PositionParameters.Length);
            Assert.AreEqual(1, f.DefaultParameters.Length);
        }

        [Test]
        public void MultipleDefaultParameters()
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.DefKeyword, "def"),
                Tuple.Create(TokenType.Identifier, "a"),
                Tuple.Create(TokenType.OpenParenthesis, "("),
                Tuple.Create(TokenType.Identifier, "b"),
                Tuple.Create(TokenType.Equals, "="),
                Tuple.Create(TokenType.Number, "5"),
                Tuple.Create(TokenType.Comma, ","),
                Tuple.Create(TokenType.Identifier, "c"),
                Tuple.Create(TokenType.Equals, "="),
                Tuple.Create(TokenType.Number, "3"),
                Tuple.Create(TokenType.Comma, ","),
                Tuple.Create(TokenType.Identifier, "d"),
                Tuple.Create(TokenType.Equals, "="),
                Tuple.Create(TokenType.Number, "1"),
                Tuple.Create(TokenType.CloseParenthesis, ")"),
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var f = FuncSetUp(GetTokens(tokens));
            Assert.AreEqual(0, f.PositionParameters.Length);
            Assert.AreEqual(3, f.DefaultParameters.Length);
        }

        [Test]
        public void DefaultAndPositionParameters()
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.DefKeyword, "def"),
                Tuple.Create(TokenType.Identifier, "a"),
                Tuple.Create(TokenType.OpenParenthesis, "("),
                Tuple.Create(TokenType.Identifier, "b"),
                Tuple.Create(TokenType.Comma, ","),
                Tuple.Create(TokenType.Identifier, "c"),
                Tuple.Create(TokenType.Equals, "="),
                Tuple.Create(TokenType.Number, "3"),
                Tuple.Create(TokenType.Comma, ","),
                Tuple.Create(TokenType.Identifier, "d"),
                Tuple.Create(TokenType.Equals, "="),
                Tuple.Create(TokenType.Number, "1"),
                Tuple.Create(TokenType.CloseParenthesis, ")"),
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var f = FuncSetUp(GetTokens(tokens));
            Assert.AreEqual(1, f.PositionParameters.Length);
            Assert.AreEqual(2, f.DefaultParameters.Length);
        }

        [Test]
        public void StatementInBody()
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.DefKeyword, "def"),
                Tuple.Create(TokenType.Identifier, "a"),
                Tuple.Create(TokenType.OpenParenthesis, "("),
                Tuple.Create(TokenType.CloseParenthesis, ")"),
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.Identifier, "a"),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var f = FuncSetUp(GetTokens(tokens));
            Assert.AreEqual(1, f.Block.Statements.Length);
        }
    }
}