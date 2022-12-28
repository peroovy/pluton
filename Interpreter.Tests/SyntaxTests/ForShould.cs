using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Interpreter.Core.Lexing;
using Interpreter.Core.Syntax;
using Interpreter.Core.Syntax.AST;
using Interpreter.Core.Syntax.AST.Expressions;
using NUnit.Framework;

namespace Interpreter.Tests.SyntaxTests
{
    [TestFixture]
    public class ForShould
    {
        private ISyntaxParser parser;
        
        [SetUp]
        public void SetUp()
        {
            parser = Core.InterpreterCore.Create().SyntaxParser;
        }

        private ForStatement ForSetUp(ImmutableArray<SyntaxToken> tokens)
        {
            var tree = parser.Parse(tokens);
            Assert.AreEqual(1, tree.Members.Length);
            Assert.IsTrue(tree.Members[0] is ForStatement);
            return (ForStatement)tree.Members[0];
        }

        private ImmutableArray<SyntaxToken> GetForTokens(
            IEnumerable<SyntaxToken> initializers,
            IEnumerable<SyntaxToken> condition,
            IEnumerable<SyntaxToken> iterators,
            IEnumerable<SyntaxToken> body)
        {
            return new[]
                {
                    new SyntaxToken(TokenTypes.ForKeyword, "for", new TextLocation()),
                    new SyntaxToken(TokenTypes.OpenParenthesis, "(", new TextLocation())
                }
                .Concat(initializers)
                .Concat(new[] {new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation())})
                .Concat(condition)
                .Concat(new[] {new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation())})
                .Concat(iterators)
                .Concat(new[] {new SyntaxToken(TokenTypes.CloseParenthesis, ")", new TextLocation())})
                .Concat(body)
                .Concat(new[] {new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation())})
                .ToImmutableArray();
        }

        [Test]
        public void SimpleFor()
        {
            var tokens = GetForTokens(
                new[]
                {
                    new SyntaxToken(TokenTypes.Identifier, "i", new TextLocation()),
                    new SyntaxToken(TokenTypes.Equals, "=", new TextLocation()),
                    new SyntaxToken(TokenTypes.Number, "0", new TextLocation())
                },
                new[]
                {
                    new SyntaxToken(TokenTypes.Identifier, "i", new TextLocation()),
                    new SyntaxToken(TokenTypes.LeftArrow, "<", new TextLocation()),
                    new SyntaxToken(TokenTypes.Number, "5", new TextLocation())
                },
                new[]
                {
                    new SyntaxToken(TokenTypes.Identifier, "i", new TextLocation()),
                    new SyntaxToken(TokenTypes.PlusEquals, "+=", new TextLocation()),
                    new SyntaxToken(TokenTypes.Number, "1", new TextLocation())
                },
                new[]
                {
                    new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                    new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation())
                }
                );
            var stmnt = ForSetUp(tokens);
            Assert.IsTrue(stmnt.Body is BlockStatement);
            Assert.AreEqual(0, ((BlockStatement)stmnt.Body).Statements.Length);
        }

        [Test]
        public void WithoutInitializers()
        {
            var tokens = GetForTokens(
                Array.Empty<SyntaxToken>(),
                new[]
                {
                    new SyntaxToken(TokenTypes.Identifier, "i", new TextLocation()),
                    new SyntaxToken(TokenTypes.LeftArrow, "<", new TextLocation()),
                    new SyntaxToken(TokenTypes.Number, "5", new TextLocation())
                },
                new[]
                {
                    new SyntaxToken(TokenTypes.Identifier, "i", new TextLocation()),
                    new SyntaxToken(TokenTypes.PlusEquals, "+=", new TextLocation()),
                    new SyntaxToken(TokenTypes.Number, "1", new TextLocation())
                },
                new[]
                {
                    new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                    new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation())
                }
                );
            var stmnt = ForSetUp(tokens);
            Assert.AreEqual(0, stmnt.Initializers.Length);
        }

        [Test]
        public void WithoutIterators()
        {
            var tokens = GetForTokens(
                new[]
                {
                    new SyntaxToken(TokenTypes.Identifier, "i", new TextLocation()),
                    new SyntaxToken(TokenTypes.Equals, "=", new TextLocation()),
                    new SyntaxToken(TokenTypes.Number, "0", new TextLocation())
                },
                new[]
                {
                    new SyntaxToken(TokenTypes.Identifier, "i", new TextLocation()),
                    new SyntaxToken(TokenTypes.LeftArrow, "<", new TextLocation()),
                    new SyntaxToken(TokenTypes.Number, "5", new TextLocation())
                },
                Array.Empty<SyntaxToken>(),
                new[]
                {
                    new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                    new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation())
                }
            );
            var stmnt = ForSetUp(tokens);
            Assert.AreEqual(0, stmnt.Iterators.Length);
        }

        [Test]
        public void BinaryInitCondIter()
        {
            var tokens = GetForTokens(
                new[]
                {
                    new SyntaxToken(TokenTypes.Identifier, "i", new TextLocation()),
                    new SyntaxToken(TokenTypes.LeftArrow, "<", new TextLocation()),
                    new SyntaxToken(TokenTypes.Number, "0", new TextLocation())
                },
                new[]
                {
                    new SyntaxToken(TokenTypes.Identifier, "i", new TextLocation()),
                    new SyntaxToken(TokenTypes.LeftArrow, "<", new TextLocation()),
                    new SyntaxToken(TokenTypes.Number, "5", new TextLocation())
                },
                new[]
                {
                    new SyntaxToken(TokenTypes.Identifier, "i", new TextLocation()),
                    new SyntaxToken(TokenTypes.RightArrow, ">", new TextLocation()),
                    new SyntaxToken(TokenTypes.Number, "1", new TextLocation())
                },
                new[]
                {
                    new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                    new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation())
                }
            );
            var stmnt = ForSetUp(tokens);
            Assert.AreEqual(1, stmnt.Initializers.Length);
            Assert.IsTrue(stmnt.Initializers[0] is BinaryExpression);
            Assert.IsTrue(stmnt.Condition is BinaryExpression);
            Assert.AreEqual(1, stmnt.Iterators.Length);
            Assert.IsTrue(stmnt.Iterators[0] is BinaryExpression);
        }

        [Test]
        public void MultipleInit()
        {
            var tokens = GetForTokens(
                new[]
                {
                    new SyntaxToken(TokenTypes.Identifier, "i", new TextLocation()),
                    new SyntaxToken(TokenTypes.Equals, "=", new TextLocation()),
                    new SyntaxToken(TokenTypes.Number, "0", new TextLocation()),
                    new SyntaxToken(TokenTypes.Comma, ",", new TextLocation()),
                    new SyntaxToken(TokenTypes.Identifier, "j", new TextLocation()),
                    new SyntaxToken(TokenTypes.Equals, "=", new TextLocation()),
                    new SyntaxToken(TokenTypes.Number, "0", new TextLocation()),
                    new SyntaxToken(TokenTypes.Comma, ",", new TextLocation()),
                    new SyntaxToken(TokenTypes.Identifier, "k", new TextLocation()),
                    new SyntaxToken(TokenTypes.Equals, "=", new TextLocation()),
                    new SyntaxToken(TokenTypes.Number, "0", new TextLocation()),
                },
                new[]
                {
                    new SyntaxToken(TokenTypes.Identifier, "i", new TextLocation()),
                    new SyntaxToken(TokenTypes.LeftArrow, "<", new TextLocation()),
                    new SyntaxToken(TokenTypes.Number, "5", new TextLocation())
                },
                new[]
                {
                    new SyntaxToken(TokenTypes.Identifier, "i", new TextLocation()),
                    new SyntaxToken(TokenTypes.PlusEquals, "+=", new TextLocation()),
                    new SyntaxToken(TokenTypes.Number, "1", new TextLocation())
                },
                new[]
                {
                    new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                    new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation())
                }
            );
            var stmnt = ForSetUp(tokens);
            Assert.AreEqual(3, stmnt.Initializers.Length);
        }

        [Test]
        public void MultipleIter()
        {
            var tokens = GetForTokens(
                new[]
                {
                    new SyntaxToken(TokenTypes.Identifier, "i", new TextLocation()),
                    new SyntaxToken(TokenTypes.Equals, "=", new TextLocation()),
                    new SyntaxToken(TokenTypes.Number, "0", new TextLocation())
                },
                new[]
                {
                    new SyntaxToken(TokenTypes.Identifier, "i", new TextLocation()),
                    new SyntaxToken(TokenTypes.LeftArrow, "<", new TextLocation()),
                    new SyntaxToken(TokenTypes.Number, "5", new TextLocation())
                },
                new[]
                {
                    new SyntaxToken(TokenTypes.Identifier, "i", new TextLocation()),
                    new SyntaxToken(TokenTypes.PlusEquals, "+=", new TextLocation()),
                    new SyntaxToken(TokenTypes.Number, "1", new TextLocation()),
                    new SyntaxToken(TokenTypes.Comma, ",", new TextLocation()),
                    
                    new SyntaxToken(TokenTypes.Identifier, "j", new TextLocation()),
                    new SyntaxToken(TokenTypes.PlusEquals, "+=", new TextLocation()),
                    new SyntaxToken(TokenTypes.Number, "1", new TextLocation()),
                    new SyntaxToken(TokenTypes.Comma, ",", new TextLocation()),
                    
                    new SyntaxToken(TokenTypes.Identifier, "k", new TextLocation()),
                    new SyntaxToken(TokenTypes.PlusEquals, "+=", new TextLocation()),
                    new SyntaxToken(TokenTypes.Number, "1", new TextLocation()),
                    new SyntaxToken(TokenTypes.Comma, ",", new TextLocation()),
                    
                    new SyntaxToken(TokenTypes.Identifier, "l", new TextLocation()),
                    new SyntaxToken(TokenTypes.PlusEquals, "+=", new TextLocation()),
                    new SyntaxToken(TokenTypes.Number, "1", new TextLocation()),
                },
                new[]
                {
                    new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                    new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation())
                }
            );
            var stmnt = ForSetUp(tokens);
            Assert.AreEqual(4, stmnt.Iterators.Length);
        }

        [Test]
        public void BooleanCondition()
        {
            var tokens = GetForTokens(
                new[]
                {
                    new SyntaxToken(TokenTypes.Identifier, "i", new TextLocation()),
                    new SyntaxToken(TokenTypes.Equals, "=", new TextLocation()),
                    new SyntaxToken(TokenTypes.Number, "0", new TextLocation())
                },
                new[]
                {
                    new SyntaxToken(TokenTypes.TrueKeyword, "true", new TextLocation()),
                },
                new[]
                {
                    new SyntaxToken(TokenTypes.Identifier, "i", new TextLocation()),
                    new SyntaxToken(TokenTypes.PlusEquals, "+=", new TextLocation()),
                    new SyntaxToken(TokenTypes.Number, "1", new TextLocation())
                },
                new[]
                {
                    new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                    new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation())
                }
            );
            var stmnt = ForSetUp(tokens);
            Assert.IsTrue(stmnt.Condition is BooleanExpression);
        }

        [Test]
        public void VariableAssignmentInitIter()
        {
            var tokens = GetForTokens(
                new[]
                {
                    new SyntaxToken(TokenTypes.Identifier, "i", new TextLocation()),
                    new SyntaxToken(TokenTypes.Equals, "=", new TextLocation()),
                    new SyntaxToken(TokenTypes.Number, "0", new TextLocation())
                },
                new[]
                {
                    new SyntaxToken(TokenTypes.Identifier, "i", new TextLocation()),
                    new SyntaxToken(TokenTypes.LeftArrow, "<", new TextLocation()),
                    new SyntaxToken(TokenTypes.Number, "5", new TextLocation())
                },
                new[]
                {
                    new SyntaxToken(TokenTypes.Identifier, "i", new TextLocation()),
                    new SyntaxToken(TokenTypes.PlusEquals, "+=", new TextLocation()),
                    new SyntaxToken(TokenTypes.Number, "1", new TextLocation())
                },
                new[]
                {
                    new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                    new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation())
                }
            );
            var stmnt = ForSetUp(tokens);
            Assert.AreEqual(1, stmnt.Initializers.Length);
            Assert.IsTrue(stmnt.Initializers[0] is VariableAssignmentExpression);
            Assert.AreEqual(1, stmnt.Iterators.Length);
            Assert.IsTrue(stmnt.Iterators[0] is VariableAssignmentExpression);
        }

        [Test]
        public void BodyStatement()
        {
            var tokens = GetForTokens(
                Array.Empty<SyntaxToken>(),
                new[]
                {
                    new SyntaxToken(TokenTypes.TrueKeyword, "true", new TextLocation()),
                },
                Array.Empty<SyntaxToken>(),
                new[]
                {
                    new SyntaxToken(TokenTypes.OpenBrace, "{", new TextLocation()),
                    new SyntaxToken(TokenTypes.BreakKeyword, "break", new TextLocation()),
                    new SyntaxToken(TokenTypes.Semicolon, ";", new TextLocation()),
                    new SyntaxToken(TokenTypes.CloseBrace, "}", new TextLocation())
                }
            );
            var stmnt = ForSetUp(tokens);
            Assert.IsTrue(stmnt.Body is BlockStatement);
            Assert.AreEqual(1, ((BlockStatement)stmnt.Body).Statements.Length);
        }
    }
}