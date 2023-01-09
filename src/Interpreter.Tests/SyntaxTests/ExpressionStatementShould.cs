using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using Core.Lexing;
using Core.Syntax;
using Core.Syntax.AST;
using Core.Syntax.AST.Expressions;
using Core.Utils.Text;
using Ninject;
using NUnit.Framework;

namespace Interpreter.Tests.SyntaxTests
{
    [TestFixture]
    public class ExpressionStatementShould
    {
        private ISyntaxParser parser;
        
        [SetUp]
        public void SetUp()
        {
            parser = Core.Interpreter.ConfigureContainer().Get<ISyntaxParser>();
        }

        private ExpressionStatement ExpressionStatementSetUp(ImmutableArray<SyntaxToken> tokens)
        {
            var tree = parser.Parse(tokens.Length == 0 ? new SourceText("") : tokens[0].Location.SourceText,
                tokens);
            Assert.IsFalse(tree.HasErrors);
            Assert.AreEqual(1, tree.Result.Members.Length);
            Assert.IsTrue(tree.Result.Members[0] is ExpressionStatement);
            return (ExpressionStatement)tree.Result.Members[0];
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
        
        [TestCase(1d, 337d, TokenType.Plus)]
        [TestCase(2d, 2d, TokenType.Star)]
        [TestCase(12d, 3d, TokenType.Slash)]
        [TestCase(3d, 0d, TokenType.Minus)]
        [TestCase(123d, 321d, TokenType.Percent)]
        public void SimpleBinary(double firstNum, double secondNum, TokenType op)
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.Number, firstNum.ToString(CultureInfo.InvariantCulture)),
                Tuple.Create(op, "+"),
                Tuple.Create(TokenType.Number, secondNum.ToString(CultureInfo.InvariantCulture)),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var exp = ExpressionStatementSetUp(GetTokens(tokens));
            Assert.IsTrue(exp.Expression is BinaryExpression);
            var binExp = (BinaryExpression)exp.Expression;
            Assert.AreEqual(op, binExp.OperatorToken.Type);
            Assert.IsTrue(binExp.Left is NumberExpression);
            Assert.IsTrue(binExp.Right is NumberExpression);
            Assert.AreEqual(firstNum, GetValue((NumberExpression)binExp.Left));
            Assert.AreEqual(secondNum, GetValue((NumberExpression)binExp.Right));
        }
        
        [Test]
        public void ComplicatedBinary()
        {
            const double firstNum = 2d;
            const double secondNum = 3d;
            const double thirdNum = 4d;
            var op1 = TokenType.Minus;
            var op2 = TokenType.Star;
            var tokens = new[]
            {
                Tuple.Create(TokenType.Number, firstNum.ToString(CultureInfo.InvariantCulture)),
                Tuple.Create(op1, "-"),
                Tuple.Create(TokenType.Number, secondNum.ToString(CultureInfo.InvariantCulture)),
                Tuple.Create(op2, "*"),
                Tuple.Create(TokenType.Number, thirdNum.ToString(CultureInfo.InvariantCulture)),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var exp = ExpressionStatementSetUp(GetTokens(tokens));
            Assert.IsTrue(exp.Expression is BinaryExpression);
            var binExp = (BinaryExpression)exp.Expression;
            Assert.AreEqual(op1, binExp.OperatorToken.Type);
            Assert.IsTrue(binExp.Left is NumberExpression);
            Assert.IsTrue(binExp.Right is BinaryExpression);
            Assert.AreEqual(firstNum, GetValue((NumberExpression)binExp.Left));
            var rightBinExp = (BinaryExpression)binExp.Right;
            Assert.AreEqual(op2, rightBinExp.OperatorToken.Type);
            Assert.IsTrue(rightBinExp.Left is NumberExpression);
            Assert.IsTrue(rightBinExp.Right is NumberExpression);
            Assert.AreEqual(secondNum, GetValue((NumberExpression)rightBinExp.Left));
            Assert.AreEqual(thirdNum, GetValue((NumberExpression)rightBinExp.Right));
        }

        [TestCase(TokenType.TrueKeyword, true)]
        [TestCase(TokenType.FalseKeyword, false)]
        public void SimpleBoolean(TokenType keyword, bool value)
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.TrueKeyword, value.ToString()),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var exp = ExpressionStatementSetUp(GetTokens(tokens));
            Assert.IsTrue(exp.Expression is BooleanExpression);
            Assert.AreEqual(value.ToString(), ((BooleanExpression)exp.Expression).Token.Text);
        }

        [TestCase("a")]
        [TestCase("NS")]
        [TestCase("camelCase")]
        [TestCase("snake_case")]
        public void SimpleFuncCall(string funcName)
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.Identifier, funcName),
                Tuple.Create(TokenType.OpenParenthesis, "("),
                Tuple.Create(TokenType.CloseParenthesis, ")"),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var exp = ExpressionStatementSetUp(GetTokens(tokens));
            Assert.IsTrue(exp.Expression is CallExpression);
        }

        [TestCase("a", 1d)]
        [TestCase("b", 5d)]
        [TestCase("test",123d)]
        public void SimpleIndexAccess(string variable, double idx)
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.Identifier, variable),
                Tuple.Create(TokenType.OpenBracket, "["),
                Tuple.Create(TokenType.Number, idx.ToString(CultureInfo.InvariantCulture)),
                Tuple.Create(TokenType.CloseBracket, "]"),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var exp = ExpressionStatementSetUp(GetTokens(tokens));
            Assert.IsTrue(exp.Expression is IndexAccessExpression);
            var idxExp = (IndexAccessExpression)exp.Expression;
            Assert.IsTrue(idxExp.IndexedExpression is VariableExpression);
            Assert.AreEqual(variable, ((VariableExpression)idxExp.IndexedExpression).Token.Text);
            Assert.IsTrue(idxExp.Index.Expression is NumberExpression);
            Assert.AreEqual(idx, GetValue((NumberExpression)idxExp.Index.Expression));
        }

        [TestCase(0d, 0d)]
        [TestCase(133d, 7d)]
        [TestCase(43d, 21d)]
        public void MultipleIndexAccess(double firstIdx, double secondIdx)
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.Identifier, "a"),
                Tuple.Create(TokenType.OpenBracket, "["),
                Tuple.Create(TokenType.Number, firstIdx.ToString(CultureInfo.InvariantCulture)),
                Tuple.Create(TokenType.CloseBracket, "]"),
                Tuple.Create(TokenType.OpenBracket, "["),
                Tuple.Create(TokenType.Number, secondIdx.ToString(CultureInfo.InvariantCulture)),
                Tuple.Create(TokenType.CloseBracket, "]"),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var exp = ExpressionStatementSetUp(GetTokens(tokens));
            Assert.IsTrue(exp.Expression is IndexAccessExpression);
            var idxExp = (IndexAccessExpression)exp.Expression;
            Assert.IsTrue(idxExp.IndexedExpression is IndexAccessExpression);
            
            var parentExp = (IndexAccessExpression)idxExp.IndexedExpression;
            Assert.IsTrue(parentExp.IndexedExpression is VariableExpression);
            Assert.IsTrue(parentExp.Index.Expression is NumberExpression);
            Assert.AreEqual(firstIdx, GetValue((NumberExpression)parentExp.Index.Expression));
            
            Assert.IsTrue(idxExp.Index.Expression is NumberExpression);
            Assert.AreEqual(secondIdx, GetValue((NumberExpression)idxExp.Index.Expression));
        }

        [TestCase(0d, 0d)]
        [TestCase(1d, 23d)]
        [TestCase(5d, 5d)]
        public void NumberIndexAssignment(double idx, double value)
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.Identifier, "a"),
                Tuple.Create(TokenType.OpenBracket, "["),
                Tuple.Create(TokenType.Number, idx.ToString(CultureInfo.InvariantCulture)),
                Tuple.Create(TokenType.CloseBracket, "]"),
                Tuple.Create(TokenType.Equals, "="),
                Tuple.Create(TokenType.Number, value.ToString(CultureInfo.InvariantCulture)),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var exp = ExpressionStatementSetUp(GetTokens(tokens));
            Assert.IsTrue(exp.Expression is IndexAssignmentExpression);
            var idxExp = (IndexAssignmentExpression)exp.Expression;
            Assert.IsTrue(idxExp.IndexedExpression is VariableExpression);
            Assert.IsTrue(idxExp.Index.Expression is NumberExpression);
            Assert.IsTrue(idxExp.Value is NumberExpression);
            Assert.AreEqual(value, GetValue((NumberExpression)idxExp.Value));
        }

        [TestCase(0d, "\"test\"")]
        [TestCase(1d, "\"test2\"")]
        [TestCase(99999d, "\"OutOfRangeException\"")]
        public void StringIndexAssignment(double idx, string value)
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.Identifier, "a"),
                Tuple.Create(TokenType.OpenBracket, "["),
                Tuple.Create(TokenType.Number, idx.ToString(CultureInfo.InvariantCulture)),
                Tuple.Create(TokenType.CloseBracket, "]"),
                Tuple.Create(TokenType.Equals, "="),
                Tuple.Create(TokenType.String, value),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var exp = ExpressionStatementSetUp(GetTokens(tokens));
            Assert.IsTrue(exp.Expression is IndexAssignmentExpression);
            var idxExp = (IndexAssignmentExpression)exp.Expression;
            Assert.IsTrue(idxExp.IndexedExpression is VariableExpression);
            Assert.IsTrue(idxExp.Index.Expression is NumberExpression);
            Assert.IsTrue(idxExp.Value is StringExpression);
            Assert.AreEqual(value, ((StringExpression)idxExp.Value).Token.Text);
        }

        [TestCase(0d, "a")]
        [TestCase(1d, "b")]
        [TestCase(123d, "test")]
        public void VariableIndexAssignment(double idx, string value)
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.Identifier, "a"),
                Tuple.Create(TokenType.OpenBracket, "["),
                Tuple.Create(TokenType.Number, idx.ToString(CultureInfo.InvariantCulture)),
                Tuple.Create(TokenType.CloseBracket, "]"),
                Tuple.Create(TokenType.Equals, "="),
                Tuple.Create(TokenType.Identifier, value),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var exp = ExpressionStatementSetUp(GetTokens(tokens));
            Assert.IsTrue(exp.Expression is IndexAssignmentExpression);
            var idxExp = (IndexAssignmentExpression)exp.Expression;
            Assert.IsTrue(idxExp.IndexedExpression is VariableExpression);
            Assert.IsTrue(idxExp.Index.Expression is NumberExpression);
            Assert.IsTrue(idxExp.Value is VariableExpression);
        }

        [TestCase(0d, 0d, 0d)]
        [TestCase(1d, 2d, 3d)]
        public void MultipleIndexAssignment(double firstIdx, double secondIdx, double value)
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.Identifier, "a"),
                Tuple.Create(TokenType.OpenBracket, "["),
                Tuple.Create(TokenType.Number, firstIdx.ToString(CultureInfo.InvariantCulture)),
                Tuple.Create(TokenType.CloseBracket, "]"),
                Tuple.Create(TokenType.OpenBracket, "["),
                Tuple.Create(TokenType.Number, secondIdx.ToString(CultureInfo.InvariantCulture)),
                Tuple.Create(TokenType.CloseBracket, "]"),
                Tuple.Create(TokenType.Equals, "="),
                Tuple.Create(TokenType.Number, value.ToString(CultureInfo.InvariantCulture)),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var exp = ExpressionStatementSetUp(GetTokens(tokens));
            Assert.IsTrue(exp.Expression is IndexAssignmentExpression);
            var idxExp = (IndexAssignmentExpression)exp.Expression;
            Assert.IsTrue(idxExp.IndexedExpression is IndexAccessExpression);
            Assert.IsTrue(idxExp.Index.Expression is NumberExpression);
            Assert.IsTrue(idxExp.Value is NumberExpression);
        }

        [TestCase(0d)]
        [TestCase(2d)]
        [TestCase(5d)]
        public void SimpleArray(double value)
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.OpenBracket, "["),
                Tuple.Create(TokenType.Number, value.ToString(CultureInfo.InvariantCulture)),
                Tuple.Create(TokenType.CloseBracket, "]"),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var exp = ExpressionStatementSetUp(GetTokens(tokens));
            Assert.IsTrue(exp.Expression is ArrayExpression);
            var listExp = (ArrayExpression)exp.Expression;
            Assert.AreEqual(1, listExp.Items.Length);
            Assert.IsTrue(listExp.Items.ToList()[0] is NumberExpression);
            Assert.AreEqual(value, GetValue((NumberExpression)listExp.Items.ToList()[0]));
        }

        [Test]
        public void MultipleItemsArray()
        {
            var values = new[] {"\"y\"", "\"u\"", "\"r\"", "\"aaa\""}.ToList();
            var tokens = new[]
            {
                Tuple.Create(TokenType.OpenBracket, "["),
                Tuple.Create(TokenType.String, values[0]),
                Tuple.Create(TokenType.Comma, ","),
                Tuple.Create(TokenType.String, values[1]),
                Tuple.Create(TokenType.Comma, ","),
                Tuple.Create(TokenType.String, values[2]),
                Tuple.Create(TokenType.Comma, ","),
                Tuple.Create(TokenType.String, values[3]),
                Tuple.Create(TokenType.CloseBracket, "]"),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var exp = ExpressionStatementSetUp(GetTokens(tokens));
            Assert.IsTrue(exp.Expression is ArrayExpression);
            var listExp = (ArrayExpression)exp.Expression;
            Assert.AreEqual(4, listExp.Items.Length);
            var listItems = listExp.Items.ToList();
            Assert.IsTrue(listItems[0] is StringExpression);
            for (var i = 0; i < 4; i++)
                Assert.AreEqual(values[i], ((StringExpression)listItems[i]).Token.Text);
        }

        [Test]
        public void EmptyArray()
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.OpenBracket, "["),
                Tuple.Create(TokenType.CloseBracket, "]"),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var exp = ExpressionStatementSetUp(GetTokens(tokens));
            Assert.IsTrue(exp.Expression is ArrayExpression);
            var listExp = (ArrayExpression)exp.Expression;
            Assert.AreEqual(0, listExp.Items.Length);
        }

        [Test]
        public void SimpleNull()
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.NullKeyword, "null"),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var exp = ExpressionStatementSetUp(GetTokens(tokens));
            Assert.IsTrue(exp.Expression is NullExpression);
        }
        
        [TestCase(5d)]
        [TestCase(123d)]
        [TestCase(24d)]
        public void SimpleNumber(double num)
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.Number, num.ToString(CultureInfo.InvariantCulture)),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var exp = ExpressionStatementSetUp(GetTokens(tokens));
            Assert.IsTrue(exp.Expression is NumberExpression);
            Assert.AreEqual(num, GetValue((NumberExpression)exp.Expression));
        }

        [TestCase("\"test\"")]
        [TestCase("\"What are you doing here? 0_o\"")]
        [TestCase("\"МАЙНКРАФТ ЭТО МОЯ ЖИЗНЬ МАЙНКРААААФТ\"")]
        public void SimpleString(string str)
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.String, str),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var exp = ExpressionStatementSetUp(GetTokens(tokens));
            Assert.IsTrue(exp.Expression is StringExpression);
            Assert.AreEqual(str, ((StringExpression)exp.Expression).Token.Text);
        }

        [TestCase(TokenType.Plus, 100500d)]
        [TestCase(TokenType.Minus, 500100d)]
        public void SimpleUnary(TokenType op, double num)
        {
            var tokens = new[]
            {
                Tuple.Create(op, "*"),
                Tuple.Create(TokenType.Number, num.ToString(CultureInfo.InvariantCulture)),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var exp = ExpressionStatementSetUp(GetTokens(tokens));
            Assert.IsTrue(exp.Expression is UnaryExpression);
            var unary = (UnaryExpression)exp.Expression;
            Assert.AreEqual(op, unary.OperatorToken.Type);
            Assert.IsTrue(unary.Operand is NumberExpression);
            Assert.AreEqual(num, GetValue((NumberExpression)unary.Operand));
        }

        [TestCase("boolean",  TokenType.TrueKeyword, typeof(BooleanExpression))]
        [TestCase("null", TokenType.NullKeyword, typeof(NullExpression))]
        [TestCase("a",  TokenType.Number, typeof(NumberExpression), "5")]
        [TestCase("str",  TokenType.String, typeof(StringExpression))]
        public void SimpleVariableAssignment(string variable, TokenType valueTokenType, Type expectedExpressionType, string value = null)
        {
            value = value ?? "somevalue";
            var tokens = new[]
            {
                Tuple.Create(TokenType.Identifier, variable),
                Tuple.Create(TokenType.Equals, "="),
                Tuple.Create(valueTokenType, value),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var exp = ExpressionStatementSetUp(GetTokens(tokens));
            Assert.IsTrue(exp.Expression is VariableAssignmentExpression);
            var ass = (VariableAssignmentExpression)exp.Expression;
            Assert.AreEqual(TokenType.Identifier, ass.Identifier.Type);
            Assert.AreEqual(variable, ass.Identifier.Text);
            Assert.AreEqual(expectedExpressionType, ass.Expression.GetType());
        }

        [Test]
        public void BinaryVariableAssignment()
        {
            const string variable = "a";
            const double firstNum = 25d;
            const double secondNum = 4d;
            var tokens = new[]
            {
                Tuple.Create(TokenType.Identifier, variable),
                Tuple.Create(TokenType.Equals, "="),
                Tuple.Create(TokenType.Number, firstNum.ToString(CultureInfo.InvariantCulture)),
                Tuple.Create(TokenType.Plus, "+"),
                Tuple.Create(TokenType.Number, secondNum.ToString(CultureInfo.InvariantCulture)),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var exp = ExpressionStatementSetUp(GetTokens(tokens));
            Assert.IsTrue(exp.Expression is VariableAssignmentExpression);
            var ass = (VariableAssignmentExpression)exp.Expression;
            Assert.AreEqual(TokenType.Identifier, ass.Identifier.Type);
            Assert.AreEqual(variable, ass.Identifier.Text);
            Assert.IsTrue(ass.Expression is BinaryExpression);
        }

        [Test]
        public void FuncCallVariableAssignment()
        {
            const string variable = "a";
            const string func = "b";
            var tokens = new[]
            {
                Tuple.Create(TokenType.Identifier, variable),
                Tuple.Create(TokenType.Equals, "="),
                Tuple.Create(TokenType.Identifier, func),
                Tuple.Create(TokenType.OpenParenthesis, "("),
                Tuple.Create(TokenType.CloseParenthesis, ")"),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var exp = ExpressionStatementSetUp(GetTokens(tokens));
            Assert.IsTrue(exp.Expression is VariableAssignmentExpression);
            var ass = (VariableAssignmentExpression)exp.Expression;
            Assert.AreEqual(TokenType.Identifier, ass.Identifier.Type);
            Assert.AreEqual(variable, ass.Identifier.Text);
            Assert.IsTrue(ass.Expression is CallExpression);
        }

        [Test]
        public void IndexAccessVariableAssignment()
        {
            const string variable = "a";
            const string parent = "b";
            const double idx = 0d;
            var tokens = new[]
            {
                Tuple.Create(TokenType.Identifier, variable),
                Tuple.Create(TokenType.Equals, "="),
                Tuple.Create(TokenType.Identifier, parent),
                Tuple.Create(TokenType.OpenBracket, "["),
                Tuple.Create(TokenType.Number, idx.ToString(CultureInfo.InvariantCulture)),
                Tuple.Create(TokenType.CloseBracket, "]"),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var exp = ExpressionStatementSetUp(GetTokens(tokens));
            Assert.IsTrue(exp.Expression is VariableAssignmentExpression);
            var ass = (VariableAssignmentExpression)exp.Expression;
            Assert.AreEqual(TokenType.Identifier, ass.Identifier.Type);
            Assert.AreEqual(variable, ass.Identifier.Text);
            Assert.IsTrue(ass.Expression is IndexAccessExpression);
        }

        [Test]
        public void ListVariableAssignment()
        {
            const string variable = "a";
            var tokens = new[]
            {
                Tuple.Create(TokenType.Identifier, variable),
                Tuple.Create(TokenType.Equals, "="),
                Tuple.Create(TokenType.OpenBracket, "["),
                Tuple.Create(TokenType.CloseBracket, "]"),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var exp = ExpressionStatementSetUp(GetTokens(tokens));
            Assert.IsTrue(exp.Expression is VariableAssignmentExpression);
            var ass = (VariableAssignmentExpression)exp.Expression;
            Assert.AreEqual(TokenType.Identifier, ass.Identifier.Type);
            Assert.AreEqual(variable, ass.Identifier.Text);
            Assert.IsTrue(ass.Expression is ArrayExpression);
        }

        [Test]
        public void ParenthesizedVariableAssignment()
        {
            const string variable = "a";
            var tokens = new[]
            {
                Tuple.Create(TokenType.Identifier, variable),
                Tuple.Create(TokenType.Equals, "="),
                Tuple.Create(TokenType.OpenParenthesis, "("),
                Tuple.Create(TokenType.Number, "1337"),
                Tuple.Create(TokenType.CloseParenthesis, ")"),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var exp = ExpressionStatementSetUp(GetTokens(tokens));
            Assert.IsTrue(exp.Expression is VariableAssignmentExpression);
            var ass = (VariableAssignmentExpression)exp.Expression;
            Assert.AreEqual(TokenType.Identifier, ass.Identifier.Type);
            Assert.AreEqual(variable, ass.Identifier.Text);
            Assert.IsTrue(ass.Expression is ParenthesizedExpression);
        }

        [Test]
        public void UnaryVariableAssignment()
        {
            const string variable = "a";
            var tokens = new[]
            {
                Tuple.Create(TokenType.Identifier, variable),
                Tuple.Create(TokenType.Equals, "="),
                Tuple.Create(TokenType.Minus, "-"),
                Tuple.Create(TokenType.Number, "1337"),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var exp = ExpressionStatementSetUp(GetTokens(tokens));
            Assert.IsTrue(exp.Expression is VariableAssignmentExpression);
            var ass = (VariableAssignmentExpression)exp.Expression;
            Assert.AreEqual(TokenType.Identifier, ass.Identifier.Type);
            Assert.AreEqual(variable, ass.Identifier.Text);
            Assert.IsTrue(ass.Expression is UnaryExpression);
        }

        [Test]
        public void VariableAssignmentVariableAssignment()
        {
            const string firstVariable = "a";
            const string secondVariable = "b";
            var tokens = new[]
            {
                Tuple.Create(TokenType.Identifier, firstVariable),
                Tuple.Create(TokenType.Equals, "="),
                Tuple.Create(TokenType.Identifier, secondVariable),
                Tuple.Create(TokenType.Equals, "="),
                Tuple.Create(TokenType.Number, "1337"),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var exp = ExpressionStatementSetUp(GetTokens(tokens));
            Assert.IsTrue(exp.Expression is VariableAssignmentExpression);
            var ass = (VariableAssignmentExpression)exp.Expression;
            Assert.AreEqual(TokenType.Identifier, ass.Identifier.Type);
            Assert.AreEqual(firstVariable, ass.Identifier.Text);
            Assert.IsTrue(ass.Expression is VariableAssignmentExpression);
        }

        [Test]
        public void VariableVariableAssignment()
        {
            const string firstVariable = "a";
            const string secondVariable = "b";
            var tokens = new[]
            {
                Tuple.Create(TokenType.Identifier, firstVariable),
                Tuple.Create(TokenType.Equals, "="),
                Tuple.Create(TokenType.Identifier, secondVariable),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var exp = ExpressionStatementSetUp(GetTokens(tokens));
            Assert.IsTrue(exp.Expression is VariableAssignmentExpression);
            var ass = (VariableAssignmentExpression)exp.Expression;
            Assert.AreEqual(TokenType.Identifier, ass.Identifier.Type);
            Assert.AreEqual(firstVariable, ass.Identifier.Text);
            Assert.IsTrue(ass.Expression is VariableExpression);
        }

        [Test]
        public void SimpleVariable()
        {
            const string variable = "a";
            var tokens = new[]
            {
                Tuple.Create(TokenType.Identifier, variable),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var exp = ExpressionStatementSetUp(GetTokens(tokens));
            Assert.IsTrue(exp.Expression is VariableExpression);
            Assert.AreEqual(variable, ((VariableExpression)exp.Expression).Token.Text);
        }
    }
}