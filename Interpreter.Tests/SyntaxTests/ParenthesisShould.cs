using System.Collections.Immutable;
using System.Globalization;
using Interpreter.Core.Syntax;
using NUnit.Framework;
using Interpreter.Core.Lexing;
using Interpreter.Core.Syntax.AST;
using Interpreter.Core.Syntax.AST.Expressions;

namespace Interpreter.Tests.SyntaxTests
{
    [TestFixture]
    public class ParenthesisShould
    {
        private ISyntaxParser parser;
        
        [SetUp]
        public void SetUp()
        {
            parser = Core.InterpreterCore.Create().SyntaxParser;
        }

        private ParenthesizedExpression SingleParenthesisExpressionCheck(SyntaxTree tree)
        {
            Assert.AreEqual(1, tree.Members.Length);
            Assert.IsTrue(tree.Members[0] is ExpressionStatement);
            var exp = (ExpressionStatement)tree.Members[0];
            Assert.IsTrue(exp.Expression is ParenthesizedExpression);
            return (ParenthesizedExpression)exp.Expression;
        }

        [Test]
        public void SimpleParenthesis()
        {
            const double expectedNum = 5d;
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.OpenParenthesis, "(", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, expectedNum.ToString(CultureInfo.InvariantCulture), new TextLocation()),
                new SyntaxToken(TokenTypes.CloseParenthesis, ")", new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var parExp = SingleParenthesisExpressionCheck(parser.Parse(tokens));
            Assert.IsTrue(parExp.InnerExpression is NumberExpression);
            var num = (NumberExpression)parExp.InnerExpression;
            Assert.AreEqual(expectedNum, num.Value);
        }

        [Test]
        public void ParenthesisSimpleExpression()
        {
            const double firstNum = 2d;
            const double secondNum = 3d;
            var braces = new[]
            {
                new SyntaxToken(TokenTypes.OpenParenthesis, "(", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, firstNum.ToString(CultureInfo.InvariantCulture), new TextLocation()),
                new SyntaxToken(TokenTypes.Plus, "+", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, secondNum.ToString(CultureInfo.InvariantCulture), new TextLocation()),
                new SyntaxToken(TokenTypes.CloseParenthesis, ")", new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var parExp = SingleParenthesisExpressionCheck(parser.Parse(braces));
            Assert.IsTrue(parExp.InnerExpression is BinaryExpression);
            var binExp = (BinaryExpression)parExp.InnerExpression;
            Assert.IsTrue(binExp.Left is NumberExpression);
            Assert.AreEqual(firstNum, ((NumberExpression)binExp.Left).Value);
            Assert.IsTrue(binExp.Right is NumberExpression);
            Assert.AreEqual(secondNum, ((NumberExpression)binExp.Right).Value);
            Assert.AreEqual(TokenTypes.Plus, binExp.OperatorToken.Type);
        }

        [Test]
        public void ParenthesisComplicatedExpression()
        {
            const double firstNum = 2d;
            const double secondNum = 3d;
            const double thirdNum = 1337d;
            var braces = new[]
            {
                new SyntaxToken(TokenTypes.OpenParenthesis, "(", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, firstNum.ToString(CultureInfo.InvariantCulture), new TextLocation()),
                new SyntaxToken(TokenTypes.Minus, "-", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, secondNum.ToString(CultureInfo.InvariantCulture), new TextLocation()),
                new SyntaxToken(TokenTypes.Star, "*", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, thirdNum.ToString(CultureInfo.InvariantCulture), new TextLocation()),
                new SyntaxToken(TokenTypes.CloseParenthesis, ")", new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var parExp = SingleParenthesisExpressionCheck(parser.Parse(braces));
            Assert.IsTrue(parExp.InnerExpression is BinaryExpression);
            var sumBinExp = (BinaryExpression)parExp.InnerExpression;
            Assert.AreEqual(TokenTypes.Minus, sumBinExp.OperatorToken.Type);
            Assert.IsTrue(sumBinExp.Left is NumberExpression);
            Assert.AreEqual(firstNum, ((NumberExpression)sumBinExp.Left).Value);
            Assert.IsTrue(sumBinExp.Right is BinaryExpression);
            var multBinExp = (BinaryExpression)sumBinExp.Right;
            Assert.AreEqual(TokenTypes.Star, multBinExp.OperatorToken.Type);
            Assert.IsTrue(multBinExp.Left is NumberExpression);
            Assert.AreEqual(secondNum, ((NumberExpression)multBinExp.Left).Value);
            Assert.IsTrue(multBinExp.Right is NumberExpression);
            Assert.AreEqual(thirdNum, ((NumberExpression)multBinExp.Right).Value);
        }
    }
}