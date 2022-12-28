using System;
using NUnit.Framework;
using Interpreter.Core.Lexing;
using Interpreter.Core.Lexing.TokenParsers;
using Interpreter.Core.Lexing.TokenParsers.Words;
using Interpreter.Core.Text;
using Interpreter.Core.Logging;

namespace Interpreter.Tests.LexingTests.TokenParsersTests
{
    [TestFixture]
    public class ParsersShould
    {
        private DoubleTerminalsParser doubleTerminalsParser;
        private NumberParser numberParser;
        private SingleTerminalsParser singleTerminalsParser;
        private StringParser stringParser;
        private WhitespaceParser whitespaceParser;
        private WordParser wordParser;
        private Logger logger;

        [SetUp]
        public void SetUp()
        {
            doubleTerminalsParser = new DoubleTerminalsParser();
            numberParser = new NumberParser();
            singleTerminalsParser = new SingleTerminalsParser();
            stringParser = new StringParser();
            whitespaceParser = new WhitespaceParser();
            wordParser = new WordParser();
            logger = new Logger();
        }
        
        public void CanParseFrom(ITokenParser parser, string text, int position, bool expected)
        {
            var line = new Line(0, text);
            Assert.AreEqual(expected, parser.CanParseFrom(line, position));
        }

        public SyntaxToken ParseCorrectly(ITokenParser parser, string text, int position, TokenTypes typeExpected)
        {
            var line = new Line(0, text);
            var actual = parser.Parse(line, position, logger);
            Assert.AreEqual(typeExpected, actual.Type);
            return actual;
        }
        
        [TestCase(">=", 0, true)]
        [TestCase("смайлик: =)", 9, false)]
        [TestCase("abc==cba", 3, true)]
        [TestCase("=", 0, false)]
        public void DoubleTerminalCanParseFrom(string text, int position, bool expected)
        {
            CanParseFrom(doubleTerminalsParser, text, position, expected);
        }
        
        [TestCase("==", 0, TokenTypes.DoubleEquals)]
        [TestCase("51!=9", 2, TokenTypes.ExclamationMarkEquals)]
        [TestCase("/= - вот такое у меня сейчас лицо", 0, TokenTypes.SlashEquals)]
        public void DoubleTerminalParseCorrectly(string text, int position, TokenTypes expected)
        {
            ParseCorrectly(doubleTerminalsParser, text, position, expected);
        }
        
        [TestCase("1", 0, true)]
        [TestCase("я число", 2, false)]
        [TestCase("n1ck", 1, true)]
        [TestCase(".0", 0, true)]
        [TestCase(".", 0, false)]
        public void NumberCanParseFrom(string text, int position, bool expected)
        {
            CanParseFrom(numberParser, text, position, expected);
        }
        
        [TestCase("1", 0, TokenTypes.Number)]
        [TestCase("51!=9", 1, TokenTypes.Number)]
        [TestCase(".5", 0, TokenTypes.Number)]
        public void NumberParseCorrectly(string text, int position, TokenTypes expected)
        {
            ParseCorrectly(numberParser, text, position, expected);
        }
        
        [TestCase("[", 0, true)]
        [TestCase("a", 0, false)]
        [TestCase("ab(de", 2, true)]
        public void SingleTerminalParseFrom(string text, int position, bool expected)
        {
            CanParseFrom(singleTerminalsParser, text, position, expected);
        }
        
        [TestCase("+", 0, TokenTypes.Plus)]
        [TestCase("51!=9", 3, TokenTypes.Equals)]
        [TestCase("\0", 0, TokenTypes.Eof)]
        public void SingleTerminalParseCorrectly(string text, int position, TokenTypes expected)
        {
            ParseCorrectly(singleTerminalsParser, text, position, expected);
        }
        
        [TestCase("\"", 0, true)]
        [TestCase("a", 0, false)]
        [TestCase("ab\"de", 2, true)]
        public void StringParseFrom(string text, int position, bool expected)
        {
            CanParseFrom(stringParser, text, position, expected);
        }
        
        //TODO: тест ""abc"" не проходит так как не видит закрывающую кавычку!
        [TestCase("\"abc\" ", "abc", 0, TokenTypes.String)]
        [TestCase("     \"test2\"   ", "test2", 5, TokenTypes.String)]
        [TestCase("abc\"def\"ghij", "def", 3, TokenTypes.String)]
        public void StringParseCorrectly(string text, string word, int position, TokenTypes expected)
        {
            var line = new Line(0, text);
            var actual = stringParser.Parse(line, position, logger);
            Assert.AreEqual(expected, actual.Type);
            Assert.AreEqual(word, actual.Text);
        }
        
        [TestCase(" ", 0, true)]
        [TestCase("a", 0, false)]
        [TestCase("ab de", 2, true)]
        public void WhitespaceParseFrom(string text, int position, bool expected)
        {
            CanParseFrom(whitespaceParser, text, position, expected);
        }
        
        [TestCase(" ", 0, TokenTypes.Space)]
        [TestCase("\n", 0, TokenTypes.LineSeparator)]
        [TestCase("abc def", 3, TokenTypes.Space)]
        public void WhitespaceParseCorrectly(string text, int position, TokenTypes expected)
        {
            ParseCorrectly(whitespaceParser, text, position, expected);
        }
        
        [TestCase("a", 0, true)]
        [TestCase("0", 0, false)]
        [TestCase("   abc   ", 3, true)]
        public void WordParseFrom(string text, int position, bool expected)
        {
            CanParseFrom(wordParser, text, position, expected);
        }
        
        [TestCase("abc", "abc", 0, TokenTypes.Identifier)]
        [TestCase("a mog us", "mog",  2, TokenTypes.Identifier)]
        [TestCase("true", "true", 0, TokenTypes.TrueKeyword)]
        [TestCase("13 and 37", "and", 3, TokenTypes.AndKeyword)]
        public void WordParseCorrectly(string text, string word, int position, TokenTypes expected)
        {
            var actual = ParseCorrectly(wordParser, text, position, expected);
            Assert.AreEqual(word, actual.Text);
        }
    }
}