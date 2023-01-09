using NUnit.Framework;
using System.Collections.Immutable;
using Core.Utils.Text;

namespace Interpreter.Tests.TextTests
{
    [TestFixture]
    public class ParserShould
    {

        [SetUp]
        public void SetUp()
        {
            
        }

        private string GetLineText(Line line)
        {
            return line.Text.Value.Substring(line.Start, line.Length);
        }

        [TestCase(new object[] {"a", "b", "c"})]
        [TestCase(new object[] {"Samkov", "Nikita", "Alekseevich"})]
        [TestCase(new object[] {"one line"})]
        [TestCase(new object[] {"two", "lines"})]
        [TestCase(new object[] {"Never", "Gonna", "Give", "You", "Up", "Never", "Gonna", "Let", "You", "Down"})]
        [TestCase(new object[] {"1", "3", "3", "7"})]
        public void CheckParse(object[] lines)
        {
            var actual = new SourceText(string.Join("\n", lines)).Lines;
            Assert.AreEqual(lines.Length, actual.Length);
            for (int i = 0; i < lines.Length; i++)
            {
                var text = (string) lines[i];
                Assert.AreEqual(text, GetLineText(actual[i]));
            }
        }

        [Test]
        public void EmptyLine()
        {
            var expected = "\0";
            var actual = new SourceText("");
            Assert.AreEqual(1, actual.Length);
            Assert.AreEqual(1, actual.Lines.Length);
            var actualLine = actual.Lines[0];
            Assert.AreEqual(string.Empty, GetLineText(actualLine));
            Assert.AreEqual(0, actualLine.Length);
        }
    }
}