using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using Core.Syntax;
using NUnit.Framework;
using Core.Lexing;
using Core.Syntax.AST;
using Core.Syntax.AST.Expressions;
using Core.Utils.Text;
using Ninject;

namespace Tests.SyntaxTests
{
    [TestFixture]
    public class ParenthesisShould
    {
        private ISyntaxParser parser;
        
        [SetUp]
        public void SetUp()
        {
            parser = Core.Interpreter.ConfigureContainer().Get<ISyntaxParser>();
        }
        
        private ImmutableArray<SyntaxToken> GetTokens(List<Tuple<TokenType, string>> values)
        {
            var text = string.Join("", values.Select(e => e.Item2));
            var stext = new SourceText(text);
            var pos = 0;
            var result = new List<SyntaxToken>();
            foreach (var value in values)
            {
                var len = value.Item2.Length;
                result.Add(new SyntaxToken(value.Item1, value.Item2, new Location(stext, pos, len)));
                pos += len;
            }

            return result.ToImmutableArray();
        }
        
        private double GetValue(NumberExpression exp)
        {
            return double.Parse(exp.Token.Text);
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
            var t = new[]
            {
                Tuple.Create(TokenType.OpenParenthesis, "("),
                Tuple.Create(TokenType.Number, expectedNum.ToString(CultureInfo.InvariantCulture)),
                Tuple.Create(TokenType.CloseParenthesis, ")"),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var tokens = GetTokens(t);
            var tree = parser.Parse(tokens.Length == 0 ? new SourceText("") : tokens[0].Location.SourceText, tokens);
            Assert.IsFalse(tree.HasErrors);
            var parExp = SingleParenthesisExpressionCheck(tree.Result);
            Assert.IsTrue(parExp.InnerExpression is NumberExpression);
            var num = (NumberExpression)parExp.InnerExpression;
            Assert.AreEqual(expectedNum, GetValue(num));
        }

        [Test]
        public void ParenthesisSimpleExpression()
        {
            const double firstNum = 2d;
            const double secondNum = 3d;
            var braces = new[]
            {
                Tuple.Create(TokenType.OpenParenthesis, "("),
                Tuple.Create(TokenType.Number, firstNum.ToString(CultureInfo.InvariantCulture)),
                Tuple.Create(TokenType.Plus, "+"),
                Tuple.Create(TokenType.Number, secondNum.ToString(CultureInfo.InvariantCulture)),
                Tuple.Create(TokenType.CloseParenthesis, ")"),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var tokens = GetTokens(braces);
            var tree = parser.Parse(tokens.Length == 0 ? new SourceText("") : tokens[0].Location.SourceText, tokens);
            Assert.IsFalse(tree.HasErrors);
            var parExp = SingleParenthesisExpressionCheck(tree.Result);
            Assert.IsTrue(parExp.InnerExpression is BinaryExpression);
            var binExp = (BinaryExpression)parExp.InnerExpression;
            Assert.IsTrue(binExp.Left is NumberExpression);
            Assert.AreEqual(firstNum, GetValue((NumberExpression)binExp.Left));
            Assert.IsTrue(binExp.Right is NumberExpression);
            Assert.AreEqual(secondNum, GetValue((NumberExpression)binExp.Right));
            Assert.AreEqual(TokenType.Plus, binExp.OperatorToken.Type);
        }

        [Test]
        public void ParenthesisComplicatedExpression()
        {
            const double firstNum = 2d;
            const double secondNum = 3d;
            const double thirdNum = 1337d;
            var braces = new[]
            {
                Tuple.Create(TokenType.OpenParenthesis, "("),
                Tuple.Create(TokenType.Number, firstNum.ToString(CultureInfo.InvariantCulture)),
                Tuple.Create(TokenType.Minus, "-"),
                Tuple.Create(TokenType.Number, secondNum.ToString(CultureInfo.InvariantCulture)),
                Tuple.Create(TokenType.Star, "*"),
                Tuple.Create(TokenType.Number, thirdNum.ToString(CultureInfo.InvariantCulture)),
                Tuple.Create(TokenType.CloseParenthesis, ")"),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var tokens = GetTokens(braces);
            var tree = parser.Parse(tokens.Length == 0 ? new SourceText("") : tokens[0].Location.SourceText, tokens);
            Assert.IsFalse(tree.HasErrors);
            var parExp = SingleParenthesisExpressionCheck(tree.Result);
            Assert.IsTrue(parExp.InnerExpression is BinaryExpression);
            var sumBinExp = (BinaryExpression)parExp.InnerExpression;
            Assert.AreEqual(TokenType.Minus, sumBinExp.OperatorToken.Type);
            Assert.IsTrue(sumBinExp.Left is NumberExpression);
            Assert.AreEqual(firstNum, GetValue((NumberExpression)sumBinExp.Left));
            Assert.IsTrue(sumBinExp.Right is BinaryExpression);
            var multBinExp = (BinaryExpression)sumBinExp.Right;
            Assert.AreEqual(TokenType.Star, multBinExp.OperatorToken.Type);
            Assert.IsTrue(multBinExp.Left is NumberExpression);
            Assert.AreEqual(secondNum, GetValue((NumberExpression)multBinExp.Left));
            Assert.IsTrue(multBinExp.Right is NumberExpression);
            Assert.AreEqual(thirdNum, GetValue((NumberExpression)multBinExp.Right));
        }
    }
}