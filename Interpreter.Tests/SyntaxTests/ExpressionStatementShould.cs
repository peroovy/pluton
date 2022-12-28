using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using Interpreter.Core.Lexing;
using Interpreter.Core.Syntax;
using Interpreter.Core.Syntax.AST;
using Interpreter.Core.Syntax.AST.Expressions;
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
            parser = Core.InterpreterCore.Create().SyntaxParser;
        }

        private ExpressionStatement ExpressionStatementSetUp(ImmutableArray<SyntaxToken> tokens)
        {
            var tree = parser.Parse(tokens);
            Assert.AreEqual(1, tree.Members.Length);
            Assert.IsTrue(tree.Members[0] is ExpressionStatement);
            return (ExpressionStatement)tree.Members[0];
        }
        
        [TestCase(1d, 337d, TokenTypes.Plus)]
        [TestCase(2d, 2d, TokenTypes.Star)]
        [TestCase(12d, 3d, TokenTypes.Slash)]
        [TestCase(3d, 0d, TokenTypes.Minus)]
        [TestCase(123d, 321d, TokenTypes.Percent)]
        public void SimpleBinary(double firstNum, double secondNum, TokenTypes op)
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.Number, firstNum.ToString(CultureInfo.InvariantCulture), new TextLocation()),
                new SyntaxToken(op, "+", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, secondNum.ToString(CultureInfo.InvariantCulture), new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var exp = ExpressionStatementSetUp(tokens);
            Assert.IsTrue(exp.Expression is BinaryExpression);
            var binExp = (BinaryExpression)exp.Expression;
            Assert.AreEqual(op, binExp.OperatorToken.Type);
            Assert.IsTrue(binExp.Left is NumberExpression);
            Assert.IsTrue(binExp.Right is NumberExpression);
            Assert.AreEqual(firstNum, ((NumberExpression)binExp.Left).Value);
            Assert.AreEqual(secondNum, ((NumberExpression)binExp.Right).Value);
        }
        
        [Test]
        public void ComplicatedBinary()
        {
            const double firstNum = 2d;
            const double secondNum = 3d;
            const double thirdNum = 4d;
            var op1 = TokenTypes.Minus;
            var op2 = TokenTypes.Star;
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.Number, firstNum.ToString(CultureInfo.InvariantCulture), new TextLocation()),
                new SyntaxToken(op1, "-", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, secondNum.ToString(CultureInfo.InvariantCulture), new TextLocation()),
                new SyntaxToken(op2, "*", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, thirdNum.ToString(CultureInfo.InvariantCulture), new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var exp = ExpressionStatementSetUp(tokens);
            Assert.IsTrue(exp.Expression is BinaryExpression);
            var binExp = (BinaryExpression)exp.Expression;
            Assert.AreEqual(op1, binExp.OperatorToken.Type);
            Assert.IsTrue(binExp.Left is NumberExpression);
            Assert.IsTrue(binExp.Right is BinaryExpression);
            Assert.AreEqual(firstNum, ((NumberExpression)binExp.Left).Value);
            var rightBinExp = (BinaryExpression)binExp.Right;
            Assert.AreEqual(op2, rightBinExp.OperatorToken.Type);
            Assert.IsTrue(rightBinExp.Left is NumberExpression);
            Assert.IsTrue(rightBinExp.Right is NumberExpression);
            Assert.AreEqual(secondNum, ((NumberExpression)rightBinExp.Left).Value);
            Assert.AreEqual(thirdNum, ((NumberExpression)rightBinExp.Right).Value);
        }

        [TestCase(TokenTypes.TrueKeyword, true)]
        [TestCase(TokenTypes.FalseKeyword, false)]
        public void SimpleBoolean(TokenTypes keyword, bool value)
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.TrueKeyword, "test", new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var exp = ExpressionStatementSetUp(tokens);
            Assert.IsTrue(exp.Expression is BooleanExpression);
            Assert.AreEqual(true, ((BooleanExpression)exp.Expression).Value);
        }

        [TestCase("a")]
        [TestCase("NS")]
        [TestCase("camelCase")]
        [TestCase("snake_case")]
        public void SimpleFuncCall(string funcName)
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.Identifier, funcName, new TextLocation()),
                new SyntaxToken(TokenTypes.OpenParenthesis, "(", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseParenthesis, ")", new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var exp = ExpressionStatementSetUp(tokens);
            Assert.IsTrue(exp.Expression is FunctionCallExpression);
            Assert.AreEqual(funcName, ((FunctionCallExpression)exp.Expression).Name.Text);
        }

        [TestCase("a", 1d)]
        [TestCase("b", 5d)]
        [TestCase("test",123d)]
        public void SimpleIndexAccess(string variable, double idx)
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.Identifier, variable, new TextLocation()),
                new SyntaxToken(TokenTypes.OpenBracket, "[", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, idx.ToString(CultureInfo.InvariantCulture), new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBracket, "]", new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var exp = ExpressionStatementSetUp(tokens);
            Assert.IsTrue(exp.Expression is IndexAccessExpression);
            var idxExp = (IndexAccessExpression)exp.Expression;
            Assert.IsTrue(idxExp.ParentExpression is VariableExpression);
            Assert.AreEqual(variable, ((VariableExpression)idxExp.ParentExpression).Name.Text);
            Assert.IsTrue(idxExp.Index.Index is NumberExpression);
            Assert.AreEqual(idx, ((NumberExpression)idxExp.Index.Index).Value);
        }

        [TestCase(0d, 0d)]
        [TestCase(133d, 7d)]
        [TestCase(43d, 21d)]
        public void MultipleIndexAccess(double firstIdx, double secondIdx)
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.Identifier, "a", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenBracket, "[", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, firstIdx.ToString(CultureInfo.InvariantCulture), new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBracket, "]", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenBracket, "[", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, secondIdx.ToString(CultureInfo.InvariantCulture), new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBracket, "]", new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var exp = ExpressionStatementSetUp(tokens);
            Assert.IsTrue(exp.Expression is IndexAccessExpression);
            var idxExp = (IndexAccessExpression)exp.Expression;
            Assert.IsTrue(idxExp.ParentExpression is IndexAccessExpression);
            
            var parentExp = (IndexAccessExpression)idxExp.ParentExpression;
            Assert.IsTrue(parentExp.ParentExpression is VariableExpression);
            Assert.IsTrue(parentExp.Index.Index is NumberExpression);
            Assert.AreEqual(firstIdx, ((NumberExpression)parentExp.Index.Index).Value);
            
            Assert.IsTrue(idxExp.Index.Index is NumberExpression);
            Assert.AreEqual(secondIdx, ((NumberExpression)idxExp.Index.Index).Value);
        }

        [TestCase(0d, 0d)]
        [TestCase(1d, 23d)]
        [TestCase(5d, 5d)]
        public void NumberIndexAssignment(double idx, double value)
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.Identifier, "a", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenBracket, "[", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, idx.ToString(CultureInfo.InvariantCulture), new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBracket, "]", new TextLocation()),
                new SyntaxToken(TokenTypes.Equals, "=", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, value.ToString(CultureInfo.InvariantCulture), new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var exp = ExpressionStatementSetUp(tokens);
            Assert.IsTrue(exp.Expression is IndexAssignmentExpression);
            var idxExp = (IndexAssignmentExpression)exp.Expression;
            Assert.IsTrue(idxExp.Expression is VariableExpression);
            Assert.IsTrue(idxExp.Index.Index is NumberExpression);
            Assert.IsTrue(idxExp.Value is NumberExpression);
            Assert.AreEqual(value, ((NumberExpression)idxExp.Value).Value);
        }

        [TestCase(0d, "\"test\"")]
        [TestCase(1d, "\"test2\"")]
        [TestCase(99999d, "\"OutOfRangeException\"")]
        public void StringIndexAssignment(double idx, string value)
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.Identifier, "a", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenBracket, "[", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, idx.ToString(CultureInfo.InvariantCulture), new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBracket, "]", new TextLocation()),
                new SyntaxToken(TokenTypes.Equals, "=", new TextLocation()),
                new SyntaxToken(TokenTypes.String, value, new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var exp = ExpressionStatementSetUp(tokens);
            Assert.IsTrue(exp.Expression is IndexAssignmentExpression);
            var idxExp = (IndexAssignmentExpression)exp.Expression;
            Assert.IsTrue(idxExp.Expression is VariableExpression);
            Assert.IsTrue(idxExp.Index.Index is NumberExpression);
            Assert.IsTrue(idxExp.Value is StringExpression);
            Assert.AreEqual(value, ((StringExpression)idxExp.Value).Value);
        }

        [TestCase(0d, "a")]
        [TestCase(1d, "b")]
        [TestCase(123d, "test")]
        public void VariableIndexAssignment(double idx, string value)
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.Identifier, "a", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenBracket, "[", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, idx.ToString(CultureInfo.InvariantCulture), new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBracket, "]", new TextLocation()),
                new SyntaxToken(TokenTypes.Equals, "=", new TextLocation()),
                new SyntaxToken(TokenTypes.Identifier, value, new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var exp = ExpressionStatementSetUp(tokens);
            Assert.IsTrue(exp.Expression is IndexAssignmentExpression);
            var idxExp = (IndexAssignmentExpression)exp.Expression;
            Assert.IsTrue(idxExp.Expression is VariableExpression);
            Assert.IsTrue(idxExp.Index.Index is NumberExpression);
            Assert.IsTrue(idxExp.Value is VariableExpression);
            Assert.AreEqual(value, ((VariableExpression)idxExp.Value).Name.Text);
        }

        [TestCase(0d, 0d, 0d)]
        [TestCase(1d, 2d, 3d)]
        public void MultipleIndexAssignment(double firstIdx, double secondIdx, double value)
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.Identifier, "a", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenBracket, "[", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, firstIdx.ToString(CultureInfo.InvariantCulture), new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBracket, "]", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenBracket, "[", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, secondIdx.ToString(CultureInfo.InvariantCulture), new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBracket, "]", new TextLocation()),
                new SyntaxToken(TokenTypes.Equals, "=", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, value.ToString(CultureInfo.InvariantCulture), new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var exp = ExpressionStatementSetUp(tokens);
            Assert.IsTrue(exp.Expression is IndexAssignmentExpression);
            var idxExp = (IndexAssignmentExpression)exp.Expression;
            Assert.IsTrue(idxExp.Expression is IndexAccessExpression);
            Assert.IsTrue(idxExp.Index.Index is NumberExpression);
            Assert.IsTrue(idxExp.Value is NumberExpression);
        }

        [TestCase(0d)]
        [TestCase(2d)]
        [TestCase(5d)]
        public void SimpleList(double value)
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.OpenBracket, "[", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, value.ToString(CultureInfo.InvariantCulture), new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBracket, "]", new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var exp = ExpressionStatementSetUp(tokens);
            Assert.IsTrue(exp.Expression is ListExpression);
            var listExp = (ListExpression)exp.Expression;
            Assert.AreEqual(1, listExp.Items.Length);
            Assert.IsTrue(listExp.Items.ToList()[0] is NumberExpression);
            Assert.AreEqual(value, ((NumberExpression)listExp.Items.ToList()[0]).Value);
        }

        [Test]
        public void MultipleItemsList()
        {
            var values = new[] {"\"y\"", "\"u\"", "\"r\"", "\"a\""}.ToList();
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.OpenBracket, "[", new TextLocation()),
                new SyntaxToken(TokenTypes.String, values[0], new TextLocation()),
                new SyntaxToken(TokenTypes.Comma, ",", new TextLocation()),
                new SyntaxToken(TokenTypes.String, values[1], new TextLocation()),
                new SyntaxToken(TokenTypes.Comma, ",", new TextLocation()),
                new SyntaxToken(TokenTypes.String, values[2], new TextLocation()),
                new SyntaxToken(TokenTypes.Comma, ",", new TextLocation()),
                new SyntaxToken(TokenTypes.String, values[3], new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBracket, "]", new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var exp = ExpressionStatementSetUp(tokens);
            Assert.IsTrue(exp.Expression is ListExpression);
            var listExp = (ListExpression)exp.Expression;
            Assert.AreEqual(4, listExp.Items.Length);
            var listItems = listExp.Items.ToList();
            Assert.IsTrue(listItems[0] is StringExpression);
            for (var i = 0; i < 4; i++)
                Assert.AreEqual(values[i], ((StringExpression)listItems[i]).Value);
        }

        [Test]
        public void EmptyList()
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.OpenBracket, "[", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBracket, "]", new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var exp = ExpressionStatementSetUp(tokens);
            Assert.IsTrue(exp.Expression is ListExpression);
            var listExp = (ListExpression)exp.Expression;
            Assert.AreEqual(0, listExp.Items.Length);
        }

        [Test]
        public void SimpleNull()
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.NullKeyword, "null", new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var exp = ExpressionStatementSetUp(tokens);
            Assert.IsTrue(exp.Expression is NullExpression);
        }
        
        [TestCase(5d)]
        [TestCase(123d)]
        [TestCase(24d)]
        public void SimpleNumber(double num)
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.Number, num.ToString(CultureInfo.InvariantCulture), new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var exp = ExpressionStatementSetUp(tokens);
            Assert.IsTrue(exp.Expression is NumberExpression);
            Assert.AreEqual(num, ((NumberExpression)exp.Expression).Value);
        }

        [TestCase("\"test\"")]
        [TestCase("\"What are you doing here? 0_o\"")]
        [TestCase("\"МАЙНКРАФТ ЭТО МОЯ ЖИЗНЬ МАЙНКРААААФТ\"")]
        public void SimpleString(string str)
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.String, str, new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var exp = ExpressionStatementSetUp(tokens);
            Assert.IsTrue(exp.Expression is StringExpression);
            Assert.AreEqual(str, ((StringExpression)exp.Expression).Value);
        }

        [TestCase(TokenTypes.Plus, 100500d)]
        [TestCase(TokenTypes.Minus, 500100d)]
        public void SimpleUnary(TokenTypes op, double num)
        {
            var tokens = new[]
            {
                new SyntaxToken(op, "*", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, num.ToString(CultureInfo.InvariantCulture), new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var exp = ExpressionStatementSetUp(tokens);
            Assert.IsTrue(exp.Expression is UnaryExpression);
            var unary = (UnaryExpression)exp.Expression;
            Assert.AreEqual(op, unary.OperatorToken.Type);
            Assert.IsTrue(unary.Operand is NumberExpression);
            Assert.AreEqual(num, ((NumberExpression)unary.Operand).Value);
        }

        [TestCase("boolean",  TokenTypes.TrueKeyword, typeof(BooleanExpression))]
        [TestCase("null", TokenTypes.NullKeyword, typeof(NullExpression))]
        [TestCase("a",  TokenTypes.Number, typeof(NumberExpression), "5")]
        [TestCase("str",  TokenTypes.String, typeof(StringExpression))]
        public void SimpleVariableAssignment(string variable, TokenTypes valueTokenType, Type expectedExpressionType, string value = null)
        {
            value = value ?? "somevalue";
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.Identifier, variable, new TextLocation()),
                new SyntaxToken(TokenTypes.Equals, "=", new TextLocation()),
                new SyntaxToken(valueTokenType, value, new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var exp = ExpressionStatementSetUp(tokens);
            Assert.IsTrue(exp.Expression is VariableAssignmentExpression);
            var ass = (VariableAssignmentExpression)exp.Expression;
            Assert.AreEqual(TokenTypes.Identifier, ass.Variable.Type);
            Assert.AreEqual(variable, ass.Variable.Text);
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
                new SyntaxToken(TokenTypes.Identifier, variable, new TextLocation()),
                new SyntaxToken(TokenTypes.Equals, "=", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, firstNum.ToString(CultureInfo.InvariantCulture), new TextLocation()),
                new SyntaxToken(TokenTypes.Plus, "+", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, secondNum.ToString(CultureInfo.InvariantCulture), new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var exp = ExpressionStatementSetUp(tokens);
            Assert.IsTrue(exp.Expression is VariableAssignmentExpression);
            var ass = (VariableAssignmentExpression)exp.Expression;
            Assert.AreEqual(TokenTypes.Identifier, ass.Variable.Type);
            Assert.AreEqual(variable, ass.Variable.Text);
            Assert.IsTrue(ass.Expression is BinaryExpression);
        }

        [Test]
        public void FuncCallVariableAssignment()
        {
            const string variable = "a";
            const string func = "b";
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.Identifier, variable, new TextLocation()),
                new SyntaxToken(TokenTypes.Equals, "=", new TextLocation()),
                new SyntaxToken(TokenTypes.Identifier, func, new TextLocation()),
                new SyntaxToken(TokenTypes.OpenParenthesis, "(", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseParenthesis, ")", new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var exp = ExpressionStatementSetUp(tokens);
            Assert.IsTrue(exp.Expression is VariableAssignmentExpression);
            var ass = (VariableAssignmentExpression)exp.Expression;
            Assert.AreEqual(TokenTypes.Identifier, ass.Variable.Type);
            Assert.AreEqual(variable, ass.Variable.Text);
            Assert.IsTrue(ass.Expression is FunctionCallExpression);
        }

        [Test]
        public void IndexAccessVariableAssignment()
        {
            const string variable = "a";
            const string parent = "b";
            const double idx = 0d;
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.Identifier, variable, new TextLocation()),
                new SyntaxToken(TokenTypes.Equals, "=", new TextLocation()),
                new SyntaxToken(TokenTypes.Identifier, parent, new TextLocation()),
                new SyntaxToken(TokenTypes.OpenBracket, "[", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, idx.ToString(CultureInfo.InvariantCulture), new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBracket, "]", new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var exp = ExpressionStatementSetUp(tokens);
            Assert.IsTrue(exp.Expression is VariableAssignmentExpression);
            var ass = (VariableAssignmentExpression)exp.Expression;
            Assert.AreEqual(TokenTypes.Identifier, ass.Variable.Type);
            Assert.AreEqual(variable, ass.Variable.Text);
            Assert.IsTrue(ass.Expression is IndexAccessExpression);
        }

        [Test]
        public void ListVariableAssignment()
        {
            const string variable = "a";
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.Identifier, variable, new TextLocation()),
                new SyntaxToken(TokenTypes.Equals, "=", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenBracket, "[", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBracket, "]", new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var exp = ExpressionStatementSetUp(tokens);
            Assert.IsTrue(exp.Expression is VariableAssignmentExpression);
            var ass = (VariableAssignmentExpression)exp.Expression;
            Assert.AreEqual(TokenTypes.Identifier, ass.Variable.Type);
            Assert.AreEqual(variable, ass.Variable.Text);
            Assert.IsTrue(ass.Expression is ListExpression);
        }

        [Test]
        public void ParenthesizedVariableAssignment()
        {
            const string variable = "a";
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.Identifier, variable, new TextLocation()),
                new SyntaxToken(TokenTypes.Equals, "=", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenParenthesis, "(", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, "1337", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseParenthesis, ")", new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var exp = ExpressionStatementSetUp(tokens);
            Assert.IsTrue(exp.Expression is VariableAssignmentExpression);
            var ass = (VariableAssignmentExpression)exp.Expression;
            Assert.AreEqual(TokenTypes.Identifier, ass.Variable.Type);
            Assert.AreEqual(variable, ass.Variable.Text);
            Assert.IsTrue(ass.Expression is ParenthesizedExpression);
        }

        [Test]
        public void UnaryVariableAssignment()
        {
            const string variable = "a";
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.Identifier, variable, new TextLocation()),
                new SyntaxToken(TokenTypes.Equals, "=", new TextLocation()),
                new SyntaxToken(TokenTypes.Minus, "-", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, "1337", new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var exp = ExpressionStatementSetUp(tokens);
            Assert.IsTrue(exp.Expression is VariableAssignmentExpression);
            var ass = (VariableAssignmentExpression)exp.Expression;
            Assert.AreEqual(TokenTypes.Identifier, ass.Variable.Type);
            Assert.AreEqual(variable, ass.Variable.Text);
            Assert.IsTrue(ass.Expression is UnaryExpression);
        }

        [Test]
        public void VariableAssignmentVariableAssignment()
        {
            const string firstVariable = "a";
            const string secondVariable = "b";
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.Identifier, firstVariable, new TextLocation()),
                new SyntaxToken(TokenTypes.Equals, "=", new TextLocation()),
                new SyntaxToken(TokenTypes.Identifier, secondVariable, new TextLocation()),
                new SyntaxToken(TokenTypes.Equals, "=", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, "1337", new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var exp = ExpressionStatementSetUp(tokens);
            Assert.IsTrue(exp.Expression is VariableAssignmentExpression);
            var ass = (VariableAssignmentExpression)exp.Expression;
            Assert.AreEqual(TokenTypes.Identifier, ass.Variable.Type);
            Assert.AreEqual(firstVariable, ass.Variable.Text);
            Assert.IsTrue(ass.Expression is VariableAssignmentExpression);
        }

        [Test]
        public void VariableVariableAssignment()
        {
            const string firstVariable = "a";
            const string secondVariable = "b";
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.Identifier, firstVariable, new TextLocation()),
                new SyntaxToken(TokenTypes.Equals, "=", new TextLocation()),
                new SyntaxToken(TokenTypes.Identifier, secondVariable, new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var exp = ExpressionStatementSetUp(tokens);
            Assert.IsTrue(exp.Expression is VariableAssignmentExpression);
            var ass = (VariableAssignmentExpression)exp.Expression;
            Assert.AreEqual(TokenTypes.Identifier, ass.Variable.Type);
            Assert.AreEqual(firstVariable, ass.Variable.Text);
            Assert.IsTrue(ass.Expression is VariableExpression);
        }

        [Test]
        public void SimpleVariable()
        {
            const string variable = "a";
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.Identifier, variable, new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var exp = ExpressionStatementSetUp(tokens);
            Assert.IsTrue(exp.Expression is VariableExpression);
            Assert.AreEqual(variable, ((VariableExpression)exp.Expression).Name.Text);
        }
    }
}