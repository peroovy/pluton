using System;
using System.Collections.Immutable;
using System.Globalization;
using Interpreter.Core.Lexing;
using Interpreter.Core.Syntax;
using Interpreter.Core.Syntax.AST;
using Interpreter.Core.Syntax.AST.Expressions;
using NUnit.Framework;

namespace Interpreter.Tests.SyntaxTests
{
    [TestFixture]
    public class IfElseShould
    {
        private ISyntaxParser parser;
        
        [SetUp]
        public void SetUp()
        {
            parser = Core.InterpreterCore.Create().SyntaxParser;
        }

        private IfStatement SingleIfSetup(ImmutableArray<SyntaxToken> tokens)
        {
            var tree = parser.Parse(tokens);
            Assert.AreEqual(1, tree.Members.Length);
            Assert.IsTrue(tree.Members[0] is IfStatement);
            return (IfStatement)tree.Members[0];
        }

        [Test]
        public void IfWithoutParenthesisCondition()
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.IfKeyword, "if", new TextLocation()),
                new SyntaxToken(TokenTypes.TrueKeyword, "true", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var ifStmnt = SingleIfSetup(tokens);
            Assert.IsTrue(ifStmnt.Condition is BooleanExpression);
            Assert.IsTrue(ifStmnt.Statement is BlockStatement);
            Assert.AreEqual(0, ((BlockStatement)ifStmnt.Statement).Statements.Length);
            Assert.IsNull(ifStmnt.ElseClause);
        }

        [Test]
        public void IfWithParenthesisCondition()
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.IfKeyword, "if", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenParenthesis, "(", new TextLocation()),
                new SyntaxToken(TokenTypes.TrueKeyword, "true", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseParenthesis, ")", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var ifStmnt = SingleIfSetup(tokens);
            Assert.IsTrue(ifStmnt.Condition is ParenthesizedExpression);
            Assert.IsTrue(((ParenthesizedExpression)ifStmnt.Condition).InnerExpression is BooleanExpression);
            Assert.IsTrue(ifStmnt.Statement is BlockStatement);
            Assert.AreEqual(0, ((BlockStatement)ifStmnt.Statement).Statements.Length);
            Assert.IsNull(ifStmnt.ElseClause);
        }

        [Test]
        public void EmptyIfElse()
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.IfKeyword, "if", new TextLocation()),
                new SyntaxToken(TokenTypes.TrueKeyword, "true", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                new SyntaxToken(TokenTypes.ElseKeyword, "else", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var ifStmnt = SingleIfSetup(tokens);
            Assert.IsTrue(ifStmnt.Condition is BooleanExpression);
            Assert.IsTrue(ifStmnt.Statement is BlockStatement);
            Assert.AreEqual(0, ((BlockStatement)ifStmnt.Statement).Statements.Length);
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
                new SyntaxToken(TokenTypes.IfKeyword, "if", new TextLocation()),
                new SyntaxToken(TokenTypes.TrueKeyword, "true", new TextLocation()),
                new SyntaxToken(TokenTypes.Identifier, variable, new TextLocation()),
                new SyntaxToken(TokenTypes.PlusEquals, "+=", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, secondNum.ToString(CultureInfo.InvariantCulture), new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var ifStmnt = SingleIfSetup(tokens);
            Assert.IsTrue(ifStmnt.Statement is ExpressionStatement);
            Assert.IsTrue(((ExpressionStatement)ifStmnt.Statement).Expression is VariableAssignmentExpression);
            var varExp = (VariableAssignmentExpression)((ExpressionStatement)ifStmnt.Statement).Expression;
            Assert.AreEqual(TokenTypes.Identifier, varExp.Variable.Type);
            Assert.AreEqual(variable, varExp.Variable.Text);
            Assert.IsTrue(varExp.Expression is BinaryExpression);
            var binExp = (BinaryExpression)varExp.Expression;
            Assert.IsTrue(binExp.Left is VariableExpression);
            Assert.AreEqual(variable, ((VariableExpression)binExp.Left).Name.Text);
            Assert.AreEqual(TokenTypes.Plus, binExp.OperatorToken.Type);
            Assert.IsTrue(binExp.Right is NumberExpression);
            Assert.AreEqual(secondNum, ((NumberExpression)binExp.Right).Value);
        }

        [Test]
        public void ElseWithExpressionStatement()
        {
            const double firstNum = 1337d;
            const string variable = "b";
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.IfKeyword, "if", new TextLocation()),
                new SyntaxToken(TokenTypes.FalseKeyword, "false", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                new SyntaxToken(TokenTypes.ElseKeyword, "else", new TextLocation()),
                new SyntaxToken(TokenTypes.Identifier, variable, new TextLocation()),
                new SyntaxToken(TokenTypes.StarEquals, "*=", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, firstNum.ToString(CultureInfo.InvariantCulture), new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var ifStmnt = SingleIfSetup(tokens);
            Assert.IsNotNull(ifStmnt.ElseClause);
            Assert.IsTrue(ifStmnt.ElseClause.Statement is ExpressionStatement);
            var exp = (ExpressionStatement)ifStmnt.ElseClause.Statement;
            Assert.IsTrue(exp.Expression is VariableAssignmentExpression);
            var varExp = (VariableAssignmentExpression)exp.Expression;
            Assert.AreEqual(TokenTypes.Identifier, varExp.Variable.Type);
            Assert.AreEqual(variable, varExp.Variable.Text);
            Assert.IsTrue(varExp.Expression is BinaryExpression);
            var binExp = (BinaryExpression)varExp.Expression;
            Assert.IsTrue(binExp.Left is VariableExpression);
            Assert.AreEqual(variable, ((VariableExpression)binExp.Left).Name.Text);
            Assert.AreEqual(TokenTypes.Star, binExp.OperatorToken.Type);
            Assert.IsTrue(binExp.Right is NumberExpression);
            Assert.AreEqual(firstNum, ((NumberExpression)binExp.Right).Value);
        }

        [TestCase(TokenTypes.BreakKeyword, typeof(BreakStatement))]
        [TestCase(TokenTypes.ContinueKeyword, typeof(ContinueStatement))]
        public void IfElseWithSingleKeywordStatement(TokenTypes keywordType, Type statementType)
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.IfKeyword, "if", new TextLocation()),
                new SyntaxToken(TokenTypes.TrueKeyword, "true", new TextLocation()),
                
                new SyntaxToken(keywordType, "test", new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                
                new SyntaxToken(TokenTypes.ElseKeyword, "else", new TextLocation()),
                
                new SyntaxToken(keywordType, "test", new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var ifStmnt = SingleIfSetup(tokens);
            Assert.AreEqual(ifStmnt.Statement.GetType() , statementType);
            Assert.IsNotNull(ifStmnt.ElseClause);
            Assert.AreEqual(ifStmnt.ElseClause.Statement.GetType(), statementType);
        }

        [Test]
        public void IfElseWithReturn()
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.IfKeyword, "if", new TextLocation()),
                new SyntaxToken(TokenTypes.TrueKeyword, "true", new TextLocation()),
                
                new SyntaxToken(TokenTypes.ReturnKeyword, "return", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, "5", new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                
                new SyntaxToken(TokenTypes.ElseKeyword, "else", new TextLocation()),
                
                new SyntaxToken(TokenTypes.ReturnKeyword, "return", new TextLocation()),
                new SyntaxToken(TokenTypes.Number, "5", new TextLocation()),
                new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var ifStmnt = SingleIfSetup(tokens);
            Assert.IsTrue(ifStmnt.Statement is ReturnStatement);
            Assert.IsNotNull(ifStmnt.ElseClause);
            Assert.IsTrue(ifStmnt.ElseClause.Statement is ReturnStatement);
        }

        [Test]
        public void IfElseWithFor()
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.IfKeyword, "if", new TextLocation()),
                new SyntaxToken(TokenTypes.TrueKeyword, "true", new TextLocation()),
                
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
                
                new SyntaxToken(TokenTypes.ElseKeyword, "else", new TextLocation()),
                
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
                
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var ifStmnt = SingleIfSetup(tokens);
            Assert.IsTrue(ifStmnt.Statement is ForStatement);
            Assert.IsNotNull(ifStmnt.ElseClause);
            Assert.IsTrue(ifStmnt.ElseClause.Statement is ForStatement);
        }

        [Test]
        public void IfElseWithFunction()
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.IfKeyword, "if", new TextLocation()),
                new SyntaxToken(TokenTypes.TrueKeyword, "true", new TextLocation()),
                
                new SyntaxToken(TokenTypes.DefKeyword, "def", new TextLocation()),
                new SyntaxToken(TokenTypes.Identifier, "a", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenParenthesis, "(", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseParenthesis, ")", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                
                new SyntaxToken(TokenTypes.ElseKeyword, "else", new TextLocation()),
                
                new SyntaxToken(TokenTypes.DefKeyword, "def", new TextLocation()),
                new SyntaxToken(TokenTypes.Identifier, "a", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenParenthesis, "(", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseParenthesis, ")", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var ifStmnt = SingleIfSetup(tokens);
            Assert.IsTrue(ifStmnt.Statement is FunctionDeclarationStatement);
            Assert.IsNotNull(ifStmnt.ElseClause);
            Assert.IsTrue(ifStmnt.ElseClause.Statement is FunctionDeclarationStatement);
        }

        [Test]
        public void IfElseWithIf()
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.IfKeyword, "if", new TextLocation()),
                new SyntaxToken(TokenTypes.TrueKeyword, "true", new TextLocation()),
                
                new SyntaxToken(TokenTypes.IfKeyword, "if", new TextLocation()),
                new SyntaxToken(TokenTypes.TrueKeyword, "true", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                new SyntaxToken(TokenTypes.ElseKeyword, "else", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                
                new SyntaxToken(TokenTypes.ElseKeyword, "else", new TextLocation()),
                
                new SyntaxToken(TokenTypes.IfKeyword, "if", new TextLocation()),
                new SyntaxToken(TokenTypes.TrueKeyword, "true", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                new SyntaxToken(TokenTypes.ElseKeyword, "else", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var ifStmnt = SingleIfSetup(tokens);
            Assert.IsTrue(ifStmnt.Statement is IfStatement);
            Assert.IsNotNull(ifStmnt.ElseClause);
            Assert.IsTrue(ifStmnt.ElseClause.Statement is IfStatement);
        }

        [Test]
        public void FirstIfDoesntStealElse()
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.IfKeyword, "if", new TextLocation()),
                new SyntaxToken(TokenTypes.TrueKeyword, "true", new TextLocation()),
                
                new SyntaxToken(TokenTypes.IfKeyword, "if", new TextLocation()),
                new SyntaxToken(TokenTypes.TrueKeyword, "true", new TextLocation()),
                
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                
                new SyntaxToken(TokenTypes.ElseKeyword, "else", new TextLocation()),
                
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var ifStmnt = SingleIfSetup(tokens);
            Assert.IsTrue(ifStmnt.Statement is IfStatement);
            Assert.IsNull(ifStmnt.ElseClause);
            Assert.IsNotNull(((IfStatement)ifStmnt.Statement).ElseClause);
        }

        [Test]
        public void DoubleIfElse()
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.IfKeyword, "if", new TextLocation()),
                new SyntaxToken(TokenTypes.TrueKeyword, "true", new TextLocation()),
                
                new SyntaxToken(TokenTypes.IfKeyword, "if", new TextLocation()),
                new SyntaxToken(TokenTypes.TrueKeyword, "true", new TextLocation()),
                
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                
                new SyntaxToken(TokenTypes.ElseKeyword, "else", new TextLocation()),
                
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                
                new SyntaxToken(TokenTypes.ElseKeyword, "else", new TextLocation()),
                
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var ifStmnt = SingleIfSetup(tokens);
            Assert.IsTrue(ifStmnt.Statement is IfStatement);
            Assert.IsNotNull(ifStmnt.ElseClause);
            Assert.IsNotNull(((IfStatement)ifStmnt.Statement).ElseClause);
        }

        [Test]
        public void IfElseWithWhile()
        {
            var tokens = new[]
            {
                new SyntaxToken(TokenTypes.IfKeyword, "if", new TextLocation()),
                new SyntaxToken(TokenTypes.TrueKeyword, "true", new TextLocation()),
                
                new SyntaxToken(TokenTypes.WhileKeyword, "while", new TextLocation()),
                new SyntaxToken(TokenTypes.TrueKeyword, "true", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                
                new SyntaxToken(TokenTypes.ElseKeyword, "else", new TextLocation()),
                
                new SyntaxToken(TokenTypes.WhileKeyword, "while", new TextLocation()),
                new SyntaxToken(TokenTypes.TrueKeyword, "true", new TextLocation()),
                new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation()),
                
                new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()),
            }.ToImmutableArray();
            var ifStmnt = SingleIfSetup(tokens);
            Assert.IsTrue(ifStmnt.Statement is WhileStatement);
            Assert.IsNotNull(ifStmnt.ElseClause);
            Assert.IsTrue(ifStmnt.ElseClause.Statement is WhileStatement);
        }
    }
}