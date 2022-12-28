using System;
using System.Collections.Immutable;
using Interpreter.Core.Lexing;
using Interpreter.Core.Syntax;
using Interpreter.Core.Syntax.AST;
using NUnit.Framework;

namespace Interpreter.Tests.SyntaxTests
{
    [TestFixture]
    public class ReturnShould
    {
        private ISyntaxParser parser;
        
        [SetUp]
        public void SetUp()
        {
            parser = Core.InterpreterCore.Create().SyntaxParser;
        }

        private ReturnStatement ReturnSetUp(ImmutableArray<SyntaxToken> tokens)
        {
            var tree = parser.Parse(tokens);
            Assert.AreEqual(1, tree.Members.Length);
            Assert.IsTrue(tree.Members[0] is ReturnStatement);
            return (ReturnStatement)tree.Members[0];
        }
        
        [Test]
        public void SimpleReturn()
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.ReturnKeyword, "return", new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var r = ReturnSetUp(tokens);
            Assert.IsNull(r.Expression);
        }
        
        [Test]
        public void ReturnStatement()
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.ReturnKeyword, "return", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, "2", new TextLocation()),
                new SyntaxToken(TokenTypes.Plus, "+", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, "2", new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var r = ReturnSetUp(tokens);
            Assert.IsNotNull(r.Expression);
        }
    }
}