using System.Collections.Immutable;
using Core.Lexing;
using Core.Syntax;
using Core.Utils.Text;
using Ninject;
using NUnit.Framework;

namespace Tests.SyntaxTests
{
    [TestFixture]
    public class EmptyLineTests
    {
        private ISyntaxParser parser;
        
        [SetUp]
        public void SetUp()
        {
            parser = Core.Interpreter.ConfigureContainer().Get<ISyntaxParser>();
        }

        [Test]
        public void EmptyLine()
        {
            var stext = new SourceText("");
            var emptyLine = 
                new[] { new SyntaxToken(TokenType.Eof, "\0", new Location(new SourceText(""), 0, 1)) }
                .ToImmutableArray();
            var tree = parser.Parse(stext, emptyLine);
            Assert.IsFalse(tree.HasErrors);
            Assert.AreEqual(0, tree.Result.Members.Length);
        }
    }
}