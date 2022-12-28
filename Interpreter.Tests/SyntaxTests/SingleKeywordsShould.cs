using System.Collections.Immutable;
using Interpreter.Core.Lexing;
using Interpreter.Core.Syntax;
using Interpreter.Core.Syntax.AST;
using NUnit.Framework;

namespace Interpreter.Tests.SyntaxTests
{
    [TestFixture]
    public class SingleKeywordsShould
    {
        private ISyntaxParser parser;
        
        [SetUp]
        public void SetUp()
        {
            parser = Core.InterpreterCore.Create().SyntaxParser;
        }
        
        [Test]
        public void SimpleBreak()
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.BreakKeyword, "break", new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var tree = parser.Parse(tokens);
            Assert.AreEqual(1, tree.Members.Length);
            Assert.IsTrue(tree.Members[0] is BreakStatement);
        }
        
        [Test]
        public void SimpleContinue()
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.ContinueKeyword, "break", new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var tree = parser.Parse(tokens);
            Assert.AreEqual(1, tree.Members.Length);
            Assert.IsTrue(tree.Members[0] is ContinueStatement);
        }
    }
}