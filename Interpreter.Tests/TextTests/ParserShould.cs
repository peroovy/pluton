using NUnit.Framework;
using System.Collections.Immutable;
using Interpreter.Core.Text;

namespace Interpreter.Tests.TextTests
{
    [TestFixture]
    public class ParserShould
    {
        private TextParser parser;

        [SetUp]
        public void SetUp()
        {
            parser = new TextParser();
        }

        [TestCase(new object[] {"a", "b", "c"})]
        [TestCase(new object[] {"Samkov", "Nikita", "Alekseevich"})]
        [TestCase(new object[] {"one line"})]
        [TestCase(new object[] {"two", "lines"})]
        [TestCase(new object[] {"Never", "Gonna", "Give", "You", "Up", "Never", "Gonna", "Let", "You", "Down"})]
        [TestCase(new object[] {"1", "3", "3", "7"})]
        public void CheckParse(object[] lines)
        {
            var actual = parser.ParseLines(string.Join("\n", lines));
            Assert.AreEqual(lines.Length, actual.Length);
            for (int i = 0; i < lines.Length; i++)
            {
                var text = (string) (i == lines.Length - 1 ? lines[i] + "\0" : lines[i]);
                Assert.AreEqual(i, actual[i].Number);
                Assert.AreEqual(text, actual[i].Value);
                Assert.AreEqual(text.Length, actual[i].Length);
            }
        }

        [Test]
        public void EmptyLine()
        {
            var expected = "\0";
            var actual = parser.ParseLines("");
            Assert.AreEqual(1, actual.Length);
            var actualLine = actual[0];
            Assert.AreEqual(0, actualLine.Number);
            Assert.AreEqual(expected, actualLine.Value);
            Assert.AreEqual(expected.Length, actualLine.Length);
        }
    }
}