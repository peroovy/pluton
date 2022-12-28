using System.Collections.Immutable;
using Interpreter.Core.Lexing;
using Interpreter.Core.Syntax;
using Interpreter.Core.Syntax.AST;
using Interpreter.Core.Syntax.AST.Expressions;
using NUnit.Framework;

namespace Interpreter.Tests.SyntaxTests
{
    [TestFixture]
    public class WhileShould
    {
        private ISyntaxParser parser;
        
        [SetUp]
        public void SetUp()
        {
            parser = Core.InterpreterCore.Create().SyntaxParser;
        }

        private WhileStatement WhileSetUp(ImmutableArray<SyntaxToken> tokens)
        {
            var tree = parser.Parse(tokens);
            Assert.AreEqual(1, tree.Members.Length);
            Assert.IsTrue(tree.Members[0] is WhileStatement);
            return (WhileStatement)tree.Members[0];
        }

        [Test]
        public void BlockEmptyBody()
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.WhileKeyword, "while", new TextLocation()),
                new SyntaxToken(TokenTypes.TrueKeyword, "true", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var w = WhileSetUp(tokens);
            Assert.IsTrue(w.Body is BlockStatement);
            Assert.AreEqual(0, ((BlockStatement)w.Body).Statements.Length);
        }

        [Test]
        public void BlockBody()
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.WhileKeyword, "while", new TextLocation()),
                new SyntaxToken(TokenTypes.TrueKeyword, "true", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.BreakKeyword, "break", new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var w = WhileSetUp(tokens);
            Assert.IsTrue(w.Body is BlockStatement);
            Assert.AreEqual(1, ((BlockStatement)w.Body).Statements.Length);
        }

        [Test]
        public void NotBlockBody()
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.WhileKeyword, "while", new TextLocation()),
                new SyntaxToken(TokenTypes.TrueKeyword, "true", new TextLocation()),
                new SyntaxToken(TokenTypes.BreakKeyword, "break", new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var w = WhileSetUp(tokens);
            Assert.IsTrue(w.Body is BreakStatement);
        }

        [Test]
        public void BoolCondition()
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.WhileKeyword, "while", new TextLocation()),
                new SyntaxToken(TokenTypes.TrueKeyword, "true", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var w = WhileSetUp(tokens);
            Assert.IsTrue(w.Condition is BooleanExpression);
        }

        [Test]
        public void BinCondition()
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.WhileKeyword, "while", new TextLocation()),
                new SyntaxToken(TokenTypes.Identifier, "a", new TextLocation()),
                new SyntaxToken(TokenTypes.LeftArrow, "<", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, "5", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var w = WhileSetUp(tokens);
            Assert.IsTrue(w.Condition is BinaryExpression);
        }
    }
}