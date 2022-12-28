using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq.Expressions;
using Interpreter.Core.Lexing;
using Interpreter.Core.Syntax;
using Interpreter.Core.Syntax.AST;
using Interpreter.Core.Syntax.AST.Expressions;
using NUnit.Framework;
using BinaryExpression = Interpreter.Core.Syntax.AST.Expressions.BinaryExpression;

namespace Interpreter.Tests.SyntaxTests
{
    [TestFixture]
    public class BlockShould
    {
        private ISyntaxParser parser;
        
        [SetUp]
        public void SetUp()
        {
            parser = Core.InterpreterCore.Create().SyntaxParser;
        }

        private BlockStatement BlockSetUp(ImmutableArray<SyntaxToken> tokens)
        {
            var tree = parser.Parse(tokens);
            Assert.AreEqual(1, tree.Members.Length);
            Assert.IsTrue(tree.Members[0] is BlockStatement);
            return (BlockStatement)tree.Members[0];
        }

        [Test]
        public void EmptyBlock()
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var block = BlockSetUp(tokens);
            Assert.AreEqual(0, block.Statements.Length);
        }

        [Test]
        public void SingleStatement()
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.BreakKeyword, "break", new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var block = BlockSetUp(tokens);
            Assert.AreEqual(1, block.Statements.Length);
        }

        [Test]
        public void MultipleStatement()
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.ContinueKeyword, "continue", new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.BreakKeyword, "break", new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.Identifier, "a", new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var block = BlockSetUp(tokens);
            Assert.AreEqual(3, block.Statements.Length);
        }

        [Test]
        public void NestedEmptyBlock()
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var block = BlockSetUp(tokens);
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
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, "5", new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var block = BlockSetUp(tokens);
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
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.Identifier, "a", new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.Identifier, "b", new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.Identifier, "c", new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.Identifier, "d", new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var block = BlockSetUp(tokens);
            Assert.AreEqual(1, block.Statements.Length);
            Assert.IsTrue(block.Statements[0] is BlockStatement);
            var nested = (BlockStatement)block.Statements[0];
            Assert.AreEqual(4, nested.Statements.Length);
        }

        [TestCase(TokenTypes.BreakKeyword, typeof(BreakStatement))]
        [TestCase(TokenTypes.ContinueKeyword, typeof(ContinueStatement))]
        public void SingleKeyword(TokenTypes tokenType, Type expected)
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(tokenType, "test", new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var block = BlockSetUp(tokens);
            Assert.AreEqual(1, block.Statements.Length);
            Assert.AreEqual(expected, block.Statements[0].GetType());
        }

        [Test]
        public void SingleIf()
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.IfKeyword, "if", new TextLocation()),
                new SyntaxToken(TokenTypes.TrueKeyword, "true", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var block = BlockSetUp(tokens);
            Assert.AreEqual(1, block.Statements.Length);
            Assert.IsTrue(block.Statements[0] is IfStatement);
            Assert.IsNull(((IfStatement)block.Statements[0]).ElseClause);
        }

        [Test]
        public void SingleIfElse()
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.IfKeyword, "if", new TextLocation()),
                new SyntaxToken(TokenTypes.TrueKeyword, "true", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                new SyntaxToken(TokenTypes.ElseKeyword, "else", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var block = BlockSetUp(tokens);
            Assert.AreEqual(1, block.Statements.Length);
            Assert.IsTrue(block.Statements[0] is IfStatement);
            Assert.IsNotNull(((IfStatement)block.Statements[0]).ElseClause);
        }

        [TestCase(2d, 3d, TokenTypes.Plus)]
        [TestCase(1, 337d, TokenTypes.Star)]
        [TestCase(7, 0d, TokenTypes.Slash)]
        [TestCase(99, 100d, TokenTypes.Minus)]
        public void SimpleExpression(double firstNum, double secondNum, TokenTypes op)
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, firstNum.ToString(CultureInfo.InvariantCulture), new TextLocation()),
                new SyntaxToken(op, "*", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, secondNum.ToString(CultureInfo.InvariantCulture), new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var block = BlockSetUp(tokens);
            Assert.AreEqual(1, block.Statements.Length);
            Assert.IsTrue(block.Statements[0] is ExpressionStatement);
            var exp = (ExpressionStatement)block.Statements[0];
            Assert.IsTrue(exp.Expression is BinaryExpression);
            var binExp = (BinaryExpression)exp.Expression;
            Assert.AreEqual(op, binExp.OperatorToken.Type);
            Assert.IsTrue(binExp.Left is NumberExpression);
            Assert.AreEqual(firstNum, ((NumberExpression)binExp.Left).Value);
            Assert.IsTrue(binExp.Right is NumberExpression);
            Assert.AreEqual(secondNum, ((NumberExpression)binExp.Right).Value);
        }

        [Test]
        public void SimpleFor()
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                
                new SyntaxToken(TokenTypes.ForKeyword, "for", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenParenthesis, "(", new TextLocation()),
                new SyntaxToken(TokenTypes.Identifier, "i", new TextLocation()),
                new SyntaxToken(TokenTypes.Equals, "=", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, "0", new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.Identifier, "i", new TextLocation()),
                new SyntaxToken(TokenTypes.LeftArrow, "<", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, "5", new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.Identifier, "i", new TextLocation()),
                new SyntaxToken(TokenTypes.PlusEquals, "+=", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, "1", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseParenthesis, ")", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var block = BlockSetUp(tokens);
            Assert.AreEqual(1, block.Statements.Length);
            Assert.IsTrue(block.Statements[0] is ForStatement);
        }

        [Test]
        public void SimpleFunc()
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                
                new SyntaxToken(TokenTypes.DefKeyword, "def", new TextLocation()),
                new SyntaxToken(TokenTypes.Identifier, "a", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenParenthesis, "(", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseParenthesis, ")", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var block = BlockSetUp(tokens);
            Assert.AreEqual(1, block.Statements.Length);
            Assert.IsTrue(block.Statements[0] is FunctionDeclarationStatement);
        }

        [Test]
        public void SimpleReturn()
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                
                new SyntaxToken(TokenTypes.ReturnKeyword, "return", new TextLocation()),
                new SyntaxToken(TokenTypes.Identifier, "a", new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var block = BlockSetUp(tokens);
            Assert.AreEqual(1, block.Statements.Length);
            Assert.IsTrue(block.Statements[0] is ReturnStatement);
        }

        [Test]
        public void SimpleWhile()
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                
                new SyntaxToken(TokenTypes.WhileKeyword, "while", new TextLocation()),
                new SyntaxToken(TokenTypes.TrueKeyword, "true", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var block = BlockSetUp(tokens);
            Assert.AreEqual(1, block.Statements.Length);
            Assert.IsTrue(block.Statements[0] is WhileStatement);
        }
    }
}