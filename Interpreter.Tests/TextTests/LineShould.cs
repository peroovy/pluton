using NUnit.Framework;
using Interpreter.Core.Text;

namespace Interpreter.Tests.TextTests
{
    [TestFixture]
    public class LineShould
    {
        private int lineNumber;
        private string lineText;
        private Line line;

        [SetUp]
        public void SetUp()
        {
            lineNumber = 1;
            lineText = "Test";
            line = new Line(lineNumber, lineText);
        }

        [Test]
        public void GettingLineNumber()
        {
            Assert.AreEqual(lineNumber, line.Number);
        }

        [Test]
        public void GettingLineValue()
        {
            Assert.AreEqual(lineText, line.Value);
        }

        [Test]
        public void GettingLineLength()
        {
            Assert.AreEqual(lineText.Length, line.Length);
        }
        
        [TestCase(2, "Test2")]
        [TestCase(128, "AbraKadabra")]
        [TestCase(1337, "NikitaSamkov")]
        [TestCase(-1, "what??")]
        [TestCase(228, "YuraPerov")]
        public void GettingLineProperties(int number, string text)
        {
            var newLine = new Line(number, text);
            Assert.AreEqual(number, newLine.Number);
            Assert.AreEqual(text, newLine.Value);
            Assert.AreEqual(text.Length, newLine.Length);
        }
    }
}