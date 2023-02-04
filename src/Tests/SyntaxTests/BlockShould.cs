using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using Core.Lexing;
using Core.Syntax;
using Core.Syntax.AST;
using Core.Utils.Text;
using Core.Syntax.AST.Expressions.Literals;
using Ninject;
using NUnit.Framework;
using BinaryExpression = Core.Syntax.AST.Expressions.BinaryExpression;

namespace Tests.SyntaxTests
{
    [TestFixture]
    public class BlockShould
    {
        private ISyntaxParser parser;
        
        [SetUp]
        public void SetUp()
        {
            parser = Core.Interpreter.ConfigureContainer().Get<ISyntaxParser>();
        }

        private BlockStatement BlockSetUp(ImmutableArray<SyntaxToken> tokens)
        {
            var state = parser.Parse(tokens.Length == 0 ? new SourceText("") : tokens[0].Location.SourceText,
                tokens);
            Assert.IsFalse(state.HasErrors);
            var tree = state.Result;
            Assert.AreEqual(1, tree.Members.Length);
            Assert.IsTrue(tree.Members[0] is BlockStatement);
            return (BlockStatement)tree.Members[0];
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
        public void EmptyBlock()
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            
            var block = BlockSetUp(GetTokens(tokens));
            Assert.AreEqual(0, block.Statements.Length);
        }

        [Test]
        public void MultipleStatement()
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.Number, "5"),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.Identifier, "b"),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.Identifier, "abc"),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var block = BlockSetUp(GetTokens(tokens));
            Assert.AreEqual(3, block.Statements.Length);
        }

        [Test]
        public void NestedEmptyBlock()
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var block = BlockSetUp(GetTokens(tokens));
            Assert.AreEqual(1, block.Statements.Length);
            Assert.IsTrue(block.Statements[0] is BlockStatement);
            var nested = (BlockStatement)block.Statements[0];
            Assert.AreEqual(0, nested.Statements.Length);
        }

        [Test]
        public void NestedWithSingleStatement()
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.Number, "5"),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var block = BlockSetUp(GetTokens(tokens));
            Assert.AreEqual(1, block.Statements.Length);
            Assert.IsTrue(block.Statements[0] is BlockStatement);
            var nested = (BlockStatement)block.Statements[0];
            Assert.AreEqual(1, nested.Statements.Length);
        }

        [Test]
        public void NestedWithMultipleStatement()
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.Identifier, "a"),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.Identifier, "b"),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.Identifier, "c"),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.Identifier, "d"),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var block = BlockSetUp(GetTokens(tokens));
            Assert.AreEqual(1, block.Statements.Length);
            Assert.IsTrue(block.Statements[0] is BlockStatement);
            var nested = (BlockStatement)block.Statements[0];
            Assert.AreEqual(4, nested.Statements.Length);
        }

        [Test]
        public void SingleIf()
        {
            
            var tokens = new[]
            {
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.IfKeyword, "if"),
                Tuple.Create(TokenType.OpenParenthesis, "("),
                Tuple.Create(TokenType.TrueKeyword, "true"),
                Tuple.Create(TokenType.CloseParenthesis, ")"),
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var block = BlockSetUp(GetTokens(tokens));
            Assert.AreEqual(1, block.Statements.Length);
            Assert.IsTrue(block.Statements[0] is IfStatement);
            Assert.IsNull(((IfStatement)block.Statements[0]).ElseClause);
        }

        [Test]
        public void SingleIfElse()
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.IfKeyword, "if"),
                Tuple.Create(TokenType.OpenParenthesis, "("),
                Tuple.Create(TokenType.TrueKeyword, "true"),
                Tuple.Create(TokenType.CloseParenthesis, ")"),
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                Tuple.Create(TokenType.ElseKeyword, "else"),
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var block = BlockSetUp(GetTokens(tokens));
            Assert.AreEqual(1, block.Statements.Length);
            Assert.IsTrue(block.Statements[0] is IfStatement);
            Assert.IsNotNull(((IfStatement)block.Statements[0]).ElseClause);
        }

        [TestCase(2d, 3d, TokenType.Plus)]
        [TestCase(1, 337d, TokenType.Star)]
        [TestCase(7, 0d, TokenType.Slash)]
        [TestCase(99, 100d, TokenType.Minus)]
        public void SimpleExpression(double firstNum, double secondNum, TokenType op)
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.Number, firstNum.ToString(CultureInfo.InvariantCulture)),
                Tuple.Create(op, "*"),
                Tuple.Create(TokenType.Number, secondNum.ToString(CultureInfo.InvariantCulture)),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var block = BlockSetUp(GetTokens(tokens));
            Assert.AreEqual(1, block.Statements.Length);
            Assert.IsTrue(block.Statements[0] is ExpressionStatement);
            var exp = (ExpressionStatement)block.Statements[0];
            Assert.IsTrue(exp.Expression is BinaryExpression);
            var binExp = (BinaryExpression)exp.Expression;
            Assert.AreEqual(op, binExp.OperatorToken.Type);
            Assert.IsTrue(binExp.Left is NumberExpression);
            Assert.AreEqual(firstNum, double.Parse(((NumberExpression)binExp.Left).Token.Text));
            Assert.IsTrue(binExp.Right is NumberExpression);
            Assert.AreEqual(secondNum, double.Parse(((NumberExpression)binExp.Right).Token.Text));
        }

        [Test]
        public void SimpleFor()
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.OpenBrace, "{"),
                
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
                
                Tuple.Create(TokenType.CloseBrace, "}"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var block = BlockSetUp(GetTokens(tokens));
            Assert.AreEqual(1, block.Statements.Length);
            Assert.IsTrue(block.Statements[0] is ForStatement);
        }

        [Test]
        public void SimpleFunc()
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.OpenBrace, "{"),
                
                Tuple.Create(TokenType.DefKeyword, "def"),
                Tuple.Create(TokenType.Identifier, "a"),
                Tuple.Create(TokenType.OpenParenthesis, "("),
                Tuple.Create(TokenType.CloseParenthesis, ")"),
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                
                Tuple.Create(TokenType.CloseBrace, "}"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var block = BlockSetUp(GetTokens(tokens));
            Assert.AreEqual(1, block.Statements.Length);
            Assert.IsTrue(block.Statements[0] is FunctionDeclarationStatement);
        }

        [Test]
        public void SimpleWhile()
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.OpenBrace, "{"),
                
                Tuple.Create(TokenType.WhileKeyword, "while"),
                Tuple.Create(TokenType.OpenParenthesis, "("),
                Tuple.Create(TokenType.TrueKeyword, "true"),
                Tuple.Create(TokenType.CloseParenthesis, ")"),
                Tuple.Create(TokenType.OpenBrace, "{"),
                Tuple.Create(TokenType.CloseBrace, "}"),
                
                Tuple.Create(TokenType.CloseBrace, "}"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var block = BlockSetUp(GetTokens(tokens));
            Assert.AreEqual(1, block.Statements.Length);
            Assert.IsTrue(block.Statements[0] is WhileStatement);
        }
    }
}