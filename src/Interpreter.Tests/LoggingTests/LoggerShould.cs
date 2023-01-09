using Core.Lexing;
using NUnit.Framework;

namespace Interpreter.Tests.LoggingTests
{
    [TestFixture]
    public class LoggerShould
    {
        /*
        private Logger logger;

        [SetUp]
        public void SetUp()
        {
            logger = new Logger();
        }

        [TestCase("a = variable", 4, "unknown variable name")]
        [TestCase("abc", 1, "b")]
        [TestCase("purple blue red orange green brown", 12, "sus")]
        public void LoggingErrors(string text, int position, string message)
        {
            logger.Reset();
            var lineNumber = 0;
            logger.Error(new TextLocation(new Line(lineNumber, text), position), 1, message);
            var actual = logger.Bucket[0];
            Assert.AreEqual(Level.Error, actual.Level);
            Assert.AreEqual($"ERROR({lineNumber}, {position}): {message}", actual.Message);
        }
        */
    }
}