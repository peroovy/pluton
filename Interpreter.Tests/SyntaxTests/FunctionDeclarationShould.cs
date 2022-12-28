using System.Collections.Immutable;
using Interpreter.Core.Lexing;
using Interpreter.Core.Syntax;
using Interpreter.Core.Syntax.AST;
using NUnit.Framework;

namespace Interpreter.Tests.SyntaxTests
{
    [TestFixture]
    public class FunctionDeclarationShould
    {
        private ISyntaxParser parser;
        
        [SetUp]
        public void SetUp()
        {
            parser = Core.InterpreterCore.Create().SyntaxParser;
        }

        private FunctionDeclarationStatement FuncSetUp(ImmutableArray<SyntaxToken> tokens)
        {
            var tree = parser.Parse(tokens);
            Assert.AreEqual(1, tree.Members.Length);
            Assert.IsTrue(tree.Members[0] is FunctionDeclarationStatement);
            return (FunctionDeclarationStatement)tree.Members[0];
        }

        [Test]
        public void SimpleFunction()
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.DefKeyword, "def", new TextLocation()),
                new SyntaxToken(TokenTypes.Identifier, "a", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenParenthesis, "(", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseParenthesis, ")", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var f = FuncSetUp(tokens);
        }

        [Test]
        public void NoParameters()
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.DefKeyword, "def", new TextLocation()),
                new SyntaxToken(TokenTypes.Identifier, "a", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenParenthesis, "(", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseParenthesis, ")", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var f = FuncSetUp(tokens);
            Assert.AreEqual(0, f.PositionParameters.Length);
            Assert.AreEqual(0, f.DefaultParameters.Length);
        }

        [Test]
        public void SingleParameter()
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.DefKeyword, "def", new TextLocation()),
                new SyntaxToken(TokenTypes.Identifier, "a", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenParenthesis, "(", new TextLocation()),
                new SyntaxToken(TokenTypes.Identifier, "b", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseParenthesis, ")", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var f = FuncSetUp(tokens);
            Assert.AreEqual(1, f.PositionParameters.Length);
            Assert.AreEqual(0, f.DefaultParameters.Length);
        }

        [Test]
        public void MultipleParameters()
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.DefKeyword, "def", new TextLocation()),
                new SyntaxToken(TokenTypes.Identifier, "a", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenParenthesis, "(", new TextLocation()),
                new SyntaxToken(TokenTypes.Identifier, "b", new TextLocation()),
                new SyntaxToken(TokenTypes.Comma, ",", new TextLocation()),
                new SyntaxToken(TokenTypes.Identifier, "c", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseParenthesis, ")", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var f = FuncSetUp(tokens);
            Assert.AreEqual(2, f.PositionParameters.Length);
            Assert.AreEqual(0, f.DefaultParameters.Length);
        }

        [Test]
        public void DefaultParameter()
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.DefKeyword, "def", new TextLocation()),
                new SyntaxToken(TokenTypes.Identifier, "a", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenParenthesis, "(", new TextLocation()),
                new SyntaxToken(TokenTypes.Identifier, "b", new TextLocation()),
                new SyntaxToken(TokenTypes.Equals, "=", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, "5", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseParenthesis, ")", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var f = FuncSetUp(tokens);
            Assert.AreEqual(0, f.PositionParameters.Length);
            Assert.AreEqual(1, f.DefaultParameters.Length);
        }

        [Test]
        public void MultipleDefaultParameters()
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.DefKeyword, "def", new TextLocation()),
                new SyntaxToken(TokenTypes.Identifier, "a", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenParenthesis, "(", new TextLocation()),
                new SyntaxToken(TokenTypes.Identifier, "b", new TextLocation()),
                new SyntaxToken(TokenTypes.Equals, "=", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, "5", new TextLocation()),
                new SyntaxToken(TokenTypes.Comma, ",", new TextLocation()),
                new SyntaxToken(TokenTypes.Identifier, "c", new TextLocation()),
                new SyntaxToken(TokenTypes.Equals, "=", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, "3", new TextLocation()),
                new SyntaxToken(TokenTypes.Comma, ",", new TextLocation()),
                new SyntaxToken(TokenTypes.Identifier, "d", new TextLocation()),
                new SyntaxToken(TokenTypes.Equals, "=", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, "1", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseParenthesis, ")", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var f = FuncSetUp(tokens);
            Assert.AreEqual(0, f.PositionParameters.Length);
            Assert.AreEqual(3, f.DefaultParameters.Length);
        }

        [Test]
        public void DefaultAndPositionParameters()
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.DefKeyword, "def", new TextLocation()),
                new SyntaxToken(TokenTypes.Identifier, "a", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenParenthesis, "(", new TextLocation()),
                new SyntaxToken(TokenTypes.Identifier, "b", new TextLocation()),
                new SyntaxToken(TokenTypes.Comma, ",", new TextLocation()),
                new SyntaxToken(TokenTypes.Identifier, "c", new TextLocation()),
                new SyntaxToken(TokenTypes.Equals, "=", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, "3", new TextLocation()),
                new SyntaxToken(TokenTypes.Comma, ",", new TextLocation()),
                new SyntaxToken(TokenTypes.Identifier, "d", new TextLocation()),
                new SyntaxToken(TokenTypes.Equals, "=", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, "1", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseParenthesis, ")", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var f = FuncSetUp(tokens);
            Assert.AreEqual(1, f.PositionParameters.Length);
            Assert.AreEqual(2, f.DefaultParameters.Length);
        }

        [Test]
        public void StatementInBody()
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.DefKeyword, "def", new TextLocation()),
                new SyntaxToken(TokenTypes.Identifier, "a", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenParenthesis, "(", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseParenthesis, ")", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.BreakKeyword, "break", new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var f = FuncSetUp(tokens);
            Assert.AreEqual(1, f.Body.Statements.Length);
        }
    }
}