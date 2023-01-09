using System.Collections.Immutable;
using Core.Lexing;
using Core.Syntax;
using Core.Syntax.AST;
using Core.Utils.Text;
using Ninject;
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
            parser = Core.Interpreter.ConfigureContainer().Get<ISyntaxParser>();
        }
        
        [Test]
        public void SimpleBreak()
        {
            var stext = new SourceText("break;");
            var tokens = new[]
            {
                new SyntaxToken(TokenType.BreakKeyword, "break", new Location(stext, 0, 5)),
                new SyntaxToken(TokenType.Semicolon, ";", new Location(stext, 5, 1)),
                new SyntaxToken(TokenType.Eof, "\0", new Location(stext, 6, 1)),
            }.ToImmutableArray();
            var tree = parser.Parse(stext, tokens);
            Assert.IsFalse(tree.HasErrors);
            Assert.AreEqual(1, tree.Result.Members.Length);
            Assert.IsTrue(tree.Result.Members[0] is BreakStatement);
        }
        
        [Test]
        public void SimpleContinue()
        {
            var stext = new SourceText("continue;");
            var tokens = new[]
            {
                new SyntaxToken(TokenType.ContinueKeyword, "continue", new Location(stext, 0, 8)),
                new SyntaxToken(TokenType.Semicolon, ";", new Location(stext, 8, 1)),
                new SyntaxToken(TokenType.Eof, "\0", new Location(stext, 9, 1)),
            }.ToImmutableArray();
            var tree = parser.Parse(stext, tokens);
            Assert.IsFalse(tree.HasErrors);
            Assert.AreEqual(1, tree.Result.Members.Length);
            Assert.IsTrue(tree.Result.Members[0] is ContinueStatement);
        }
    }
}