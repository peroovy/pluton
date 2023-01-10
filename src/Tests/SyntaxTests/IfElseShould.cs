using System;
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

namespace Tests.SyntaxTests
{
    [TestFixture]
    public class IfElseShould
    {
        private ISyntaxParser parser;
        
        [SetUp]
        public void SetUp()
        {
            parser = Core.Interpreter.ConfigureContainer().Get<ISyntaxParser>();
        }

        private IfStatement SingleIfSetup(ImmutableArray<SyntaxToken> tokens)
        {
            var tree = parser.Parse(tokens.Length == 0 ? new SourceText("") : tokens[0].Location.SourceText,
                tokens);
            Assert.IsFalse(tree.HasErrors);
            Assert.AreEqual(1, tree.Result.Members.Length);
            Assert.IsTrue(tree.Result.Members[0] is IfStatement);
            return (IfStatement)tree.Result.Members[0];
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

        [Test]
        public void IfWithParenthesisCondition()
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.IfKeyword, "if"),
                Tuple.Create(TokenType.OpenParenthesis, "("),
                Tuple.Create(TokenType.TrueKeyword, "true"),
                Tuple.Create(TokenType.CloseParenthesis, ")"),
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var ifStmnt = SingleIfSetup(GetTokens(tokens));
            Assert.IsTrue(ifStmnt.Condition is BooleanExpression);
            Assert.IsTrue(ifStmnt.ThenStatement is BlockStatement);
            Assert.AreEqual(0, ((BlockStatement)ifStmnt.ThenStatement).Statements.Length);
            Assert.IsNull(ifStmnt.ElseClause);
        }

        [Test]
        public void EmptyIfElse()
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.IfKeyword, "if"),
                Tuple.Create(TokenType.OpenParenthesis, "("),
                Tuple.Create(TokenType.TrueKeyword, "true"),
                Tuple.Create(TokenType.CloseParenthesis, ")"),
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                Tuple.Create(TokenType.ElseKeyword, "else"),
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var ifStmnt = SingleIfSetup(GetTokens(tokens));
            Assert.IsTrue(ifStmnt.Condition is BooleanExpression);
            Assert.IsTrue(ifStmnt.ThenStatement is BlockStatement);
            Assert.AreEqual(0, ((BlockStatement)ifStmnt.ThenStatement).Statements.Length);
            Assert.IsNotNull(ifStmnt.ElseClause);
            Assert.IsTrue(ifStmnt.ElseClause.Statement is BlockStatement);
            Assert.AreEqual(0, ((BlockStatement)ifStmnt.ElseClause.Statement).Statements.Length);
        }

        [Test]
        public void IfWithExpressionStatement()
        {
            const double secondNum = 1d;
            const string variable = "a";
            var tokens = new[]
            {
                Tuple.Create(TokenType.IfKeyword, "if"),
                Tuple.Create(TokenType.OpenParenthesis, "("),
                Tuple.Create(TokenType.TrueKeyword, "true"),
                Tuple.Create(TokenType.CloseParenthesis, ")"),
                Tuple.Create(TokenType.Identifier, variable),
                Tuple.Create(TokenType.PlusEquals, "+="),
                Tuple.Create(TokenType.Number, secondNum.ToString(CultureInfo.InvariantCulture)),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var ifStmnt = SingleIfSetup(GetTokens(tokens));
            Assert.IsTrue(ifStmnt.ThenStatement is ExpressionStatement);
            Assert.IsTrue(((ExpressionStatement)ifStmnt.ThenStatement).Expression is VariableAssignmentExpression);
            var varExp = (VariableAssignmentExpression)((ExpressionStatement)ifStmnt.ThenStatement).Expression;
            Assert.AreEqual(TokenType.Identifier, varExp.Identifier.Type);
            Assert.AreEqual(variable, varExp.Identifier.Text);
            Assert.IsTrue(varExp.Expression is BinaryExpression);
            var binExp = (BinaryExpression)varExp.Expression;
            Assert.IsTrue(binExp.Left is VariableExpression);
            Assert.AreEqual(variable, ((VariableExpression)binExp.Left).Token.Text);
            Assert.AreEqual(TokenType.Plus, binExp.OperatorToken.Type);
            Assert.IsTrue(binExp.Right is NumberExpression);
            Assert.AreEqual(secondNum, double.Parse(((NumberExpression)binExp.Right).Token.Text));
        }

        [Test]
        public void ElseWithExpressionStatement()
        {
            const double firstNum = 1337d;
            const string variable = "b";
            var tokens = new[]
            {
                Tuple.Create(TokenType.IfKeyword, "if"),
                Tuple.Create(TokenType.OpenParenthesis, "("),
                Tuple.Create(TokenType.FalseKeyword, "false"),
                Tuple.Create(TokenType.CloseParenthesis, ")"),
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                Tuple.Create(TokenType.ElseKeyword, "else"),
                Tuple.Create(TokenType.Identifier, variable),
                Tuple.Create(TokenType.StarEquals, "*="),
                Tuple.Create(TokenType.Number, firstNum.ToString(CultureInfo.InvariantCulture)),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var ifStmnt = SingleIfSetup(GetTokens(tokens));
            Assert.IsNotNull(ifStmnt.ElseClause);
            Assert.IsTrue(ifStmnt.ElseClause.Statement is ExpressionStatement);
            var exp = (ExpressionStatement)ifStmnt.ElseClause.Statement;
            Assert.IsTrue(exp.Expression is VariableAssignmentExpression);
            var varExp = (VariableAssignmentExpression)exp.Expression;
            Assert.AreEqual(TokenType.Identifier, varExp.Identifier.Type);
            Assert.AreEqual(variable, varExp.Identifier.Text);
            Assert.IsTrue(varExp.Expression is BinaryExpression);
            var binExp = (BinaryExpression)varExp.Expression;
            Assert.IsTrue(binExp.Left is VariableExpression);
            Assert.AreEqual(variable, ((VariableExpression)binExp.Left).Token.Text);
            Assert.AreEqual(TokenType.Star, binExp.OperatorToken.Type);
            Assert.IsTrue(binExp.Right is NumberExpression);
            Assert.AreEqual(firstNum, double.Parse(((NumberExpression)binExp.Right).Token.Text));
        }

        [Test]
        public void IfElseWithFor()
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.IfKeyword, "if"),
                Tuple.Create(TokenType.OpenParenthesis, "("),
                Tuple.Create(TokenType.TrueKeyword, "true"),
                Tuple.Create(TokenType.CloseParenthesis, ")"),
                
                Tuple.Create(TokenType.ForKeyword, "for"),
                Tuple.Create(TokenType.OpenParenthesis, "("),
                Tuple.Create(TokenType.Identifier, "i"),
                Tuple.Create(TokenType.Equals, "="),
                Tuple.Create(TokenType.Number, "0"),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.Identifier, "i"),
                Tuple.Create(TokenType.LeftArrow, "<"),
                Tuple.Create(TokenType.Number, "5"),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.Identifier, "i"),
                Tuple.Create(TokenType.PlusEquals, "+="),
                Tuple.Create(TokenType.Number, "1"),
                Tuple.Create(TokenType.CloseParenthesis, ")"),
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                
                Tuple.Create(TokenType.ElseKeyword, "else"),
                
                Tuple.Create(TokenType.ForKeyword, "for"),
                Tuple.Create(TokenType.OpenParenthesis, "("),
                Tuple.Create(TokenType.Identifier, "i"),
                Tuple.Create(TokenType.Equals, "="),
                Tuple.Create(TokenType.Number, "0"),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.Identifier, "i"),
                Tuple.Create(TokenType.LeftArrow, "<"),
                Tuple.Create(TokenType.Number, "5"),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.Identifier, "i"),
                Tuple.Create(TokenType.PlusEquals, "+="),
                Tuple.Create(TokenType.Number, "1"),
                Tuple.Create(TokenType.CloseParenthesis, ")"),
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var ifStmnt = SingleIfSetup(GetTokens(tokens));
            Assert.IsTrue(ifStmnt.ThenStatement is ForStatement);
            Assert.IsNotNull(ifStmnt.ElseClause);
            Assert.IsTrue(ifStmnt.ElseClause.Statement is ForStatement);
        }

        [Test]
        public void IfElseWithFunction()
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.IfKeyword, "if"),
                Tuple.Create(TokenType.OpenParenthesis, "("),
                Tuple.Create(TokenType.TrueKeyword, "true"),
                Tuple.Create(TokenType.CloseParenthesis, ")"),
                
                Tuple.Create(TokenType.DefKeyword, "def"),
                Tuple.Create(TokenType.Identifier, "a"),
                Tuple.Create(TokenType.OpenParenthesis, "("),
                Tuple.Create(TokenType.CloseParenthesis, ")"),
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                
                Tuple.Create(TokenType.ElseKeyword, "else"),
                
                Tuple.Create(TokenType.DefKeyword, "def"),
                Tuple.Create(TokenType.Identifier, "a"),
                Tuple.Create(TokenType.OpenParenthesis, "("),
                Tuple.Create(TokenType.CloseParenthesis, ")"),
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var ifStmnt = SingleIfSetup(GetTokens(tokens));
            Assert.IsTrue(ifStmnt.ThenStatement is FunctionDeclarationStatement);
            Assert.IsNotNull(ifStmnt.ElseClause);
            Assert.IsTrue(ifStmnt.ElseClause.Statement is FunctionDeclarationStatement);
        }

        [Test]
        public void IfElseWithIf()
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.IfKeyword, "if"),
                Tuple.Create(TokenType.OpenParenthesis, "("),
                Tuple.Create(TokenType.TrueKeyword, "true"),
                Tuple.Create(TokenType.CloseParenthesis, ")"),
                
                Tuple.Create(TokenType.IfKeyword, "if"),
                Tuple.Create(TokenType.OpenParenthesis, "("),
                Tuple.Create(TokenType.TrueKeyword, "true"),
                Tuple.Create(TokenType.CloseParenthesis, ")"),
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                Tuple.Create(TokenType.ElseKeyword, "else"),
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                
                Tuple.Create(TokenType.ElseKeyword, "else"),
                
                Tuple.Create(TokenType.IfKeyword, "if"),
                Tuple.Create(TokenType.OpenParenthesis, "("),
                Tuple.Create(TokenType.TrueKeyword, "true"),
                Tuple.Create(TokenType.CloseParenthesis, ")"),
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                Tuple.Create(TokenType.ElseKeyword, "else"),
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var ifStmnt = SingleIfSetup(GetTokens(tokens));
            Assert.IsTrue(ifStmnt.ThenStatement is IfStatement);
            Assert.IsNotNull(ifStmnt.ElseClause);
            Assert.IsTrue(ifStmnt.ElseClause.Statement is IfStatement);
        }

        [Test]
        public void FirstIfDoesntStealElse()
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.IfKeyword, "if"),
                Tuple.Create(TokenType.OpenParenthesis, "("),
                Tuple.Create(TokenType.TrueKeyword, "true"),
                Tuple.Create(TokenType.CloseParenthesis, ")"),
                
                Tuple.Create(TokenType.IfKeyword, "if"),
                Tuple.Create(TokenType.OpenParenthesis, "("),
                Tuple.Create(TokenType.TrueKeyword, "true"),
                Tuple.Create(TokenType.CloseParenthesis, ")"),
                
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                
                Tuple.Create(TokenType.ElseKeyword, "else"),
                
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var ifStmnt = SingleIfSetup(GetTokens(tokens));
            Assert.IsTrue(ifStmnt.ThenStatement is IfStatement);
            Assert.IsNull(ifStmnt.ElseClause);
            Assert.IsNotNull(((IfStatement)ifStmnt.ThenStatement).ElseClause);
        }

        [Test]
        public void DoubleIfElse()
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.IfKeyword, "if"),
                Tuple.Create(TokenType.OpenParenthesis, "("),
                Tuple.Create(TokenType.TrueKeyword, "true"),
                Tuple.Create(TokenType.CloseParenthesis, ")"),
                
                Tuple.Create(TokenType.IfKeyword, "if"),
                Tuple.Create(TokenType.OpenParenthesis, "("),
                Tuple.Create(TokenType.TrueKeyword, "true"),
                Tuple.Create(TokenType.CloseParenthesis, ")"),
                
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                
                Tuple.Create(TokenType.ElseKeyword, "else"),
                
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                
                Tuple.Create(TokenType.ElseKeyword, "else"),
                
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var ifStmnt = SingleIfSetup(GetTokens(tokens));
            Assert.IsTrue(ifStmnt.ThenStatement is IfStatement);
            Assert.IsNotNull(ifStmnt.ElseClause);
            Assert.IsNotNull(((IfStatement)ifStmnt.ThenStatement).ElseClause);
        }

        [Test]
        public void IfElseWithWhile()
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.IfKeyword, "if"),
                Tuple.Create(TokenType.OpenParenthesis, "("),
                Tuple.Create(TokenType.TrueKeyword, "true"),
                Tuple.Create(TokenType.CloseParenthesis, ")"),
                
                Tuple.Create(TokenType.WhileKeyword, "while"),
                Tuple.Create(TokenType.OpenParenthesis, "("),
                Tuple.Create(TokenType.TrueKeyword, "true"),
                Tuple.Create(TokenType.CloseParenthesis, ")"),
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                
                Tuple.Create(TokenType.ElseKeyword, "else"),
                
                Tuple.Create(TokenType.WhileKeyword, "while"),
                Tuple.Create(TokenType.OpenParenthesis, "("),
                Tuple.Create(TokenType.TrueKeyword, "true"),
                Tuple.Create(TokenType.CloseParenthesis, ")"),
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var ifStmnt = SingleIfSetup(GetTokens(tokens));
            Assert.IsTrue(ifStmnt.ThenStatement is WhileStatement);
            Assert.IsNotNull(ifStmnt.ElseClause);
            Assert.IsTrue(ifStmnt.ElseClause.Statement is WhileStatement);
        }
    }
}