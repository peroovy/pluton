using System;
using NUnit.Framework;
using Core.Lexing;
using Core.Lexing.TokenParsers;
using Core.Utils.Diagnostic;
using Core.Utils.Text;
using Ninject;

namespace Interpreter.Tests.LexingTests.TokenParsersTests
{
    [TestFixture]
    public class ParsersShould
    {
        private DoubleTerminalParser doubleTerminalsParser;
        private NumberParser numberParser;
        private SingleTerminalParser singleTerminalsParser;
        private StringParser stringParser;
        private SpaceParser whitespaceParser;
        private WordTerminalParser wordParser;
        private DiagnosticBag dBag;

        [SetUp]
        public void SetUp()
        {
            var container = Core.Interpreter.ConfigureContainer();

            doubleTerminalsParser = container.Get<DoubleTerminalParser>();
            numberParser = container.Get<NumberParser>();
            singleTerminalsParser = container.Get<SingleTerminalParser>();
            stringParser = container.Get<StringParser>();
            whitespaceParser = container.Get<SpaceParser>();
            wordParser = container.Get<WordTerminalParser>();
            dBag = container.Get<DiagnosticBag>();
        }

        public SyntaxToken ParseCorrectly(ITokenParser parser, string text, int position, TokenType typeExpected)
        {
            var actual = parser.TryParse(new SourceText(text), position, dBag);
            Assert.AreEqual(typeExpected, actual.Type);
            return actual;
        }
        
        [TestCase("==", 0, TokenType.DoubleEquals)]
        [TestCase("51!=9", 2, TokenType.ExclamationMarkEquals)]
        [TestCase("/= - вот такое у меня сейчас лицо", 0, TokenType.SlashEquals)]
        public void DoubleTerminalParseCorrectly(string text, int position, TokenType expected)
        {
            ParseCorrectly(doubleTerminalsParser, text, position, expected);
        }
        
        [TestCase("1", 0, TokenType.Number)]
        [TestCase("51!=9", 1, TokenType.Number)]
        [TestCase(".5", 0, TokenType.Number)]
        public void NumberParseCorrectly(string text, int position, TokenType expected)
        {
            ParseCorrectly(numberParser, text, position, expected);
        }
        
        [TestCase("+", 0, TokenType.Plus)]
        [TestCase("51!=9", 3, TokenType.Equals)]
        [TestCase("\0", 0, TokenType.Eof)]
        public void SingleTerminalParseCorrectly(string text, int position, TokenType expected)
        {
            ParseCorrectly(singleTerminalsParser, text, position, expected);
        }
        
        [TestCase("\"abc\" ", "abc", 0, TokenType.String)]
        [TestCase("     \"test2\"   ", "test2", 5, TokenType.String)]
        [TestCase("abc\"def\"ghij", "def", 3, TokenType.String)]
        public void StringParseCorrectly(string text, string word, int position, TokenType expected)
        {
            var actual = stringParser.TryParse(new SourceText(text), position, dBag);
            Assert.AreEqual(expected, actual.Type);
            Assert.AreEqual(word, actual.Text);
        }
        
        [TestCase(" ", 0, TokenType.Space)]
        [TestCase("abc def", 3, TokenType.Space)]
        public void WhitespaceParseCorrectly(string text, int position, TokenType expected)
        {
            ParseCorrectly(whitespaceParser, text, position, expected);
        }
        
        //TODO: LineBreakParser
        
        [TestCase("abc", "abc", 0, TokenType.Identifier)]
        [TestCase("a mog us", "mog",  2, TokenType.Identifier)]
        [TestCase("true", "true", 0, TokenType.TrueKeyword)]
        [TestCase("13 and 37", "and", 3, TokenType.AndKeyword)]
        public void WordParseCorrectly(string text, string word, int position, TokenType expected)
        {
            var actual = ParseCorrectly(wordParser, text, position, expected);
            Assert.AreEqual(word, actual.Text);
        }
    }
}