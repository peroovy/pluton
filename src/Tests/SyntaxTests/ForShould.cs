using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
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
    public class ForShould
    {
        private ISyntaxParser parser;
        
        [SetUp]
        public void SetUp()
        {
            parser = Core.Interpreter.ConfigureContainer().Get<ISyntaxParser>();
        }

        private ForStatement ForSetUp(ImmutableArray<SyntaxToken> tokens)
        {
            var tree = parser.Parse(tokens.Length == 0 ? new SourceText("") : tokens[0].Location.SourceText, 
                tokens);
            Assert.IsFalse(tree.HasErrors);
            Assert.AreEqual(1, tree.Result.Members.Length);
            Assert.IsTrue(tree.Result.Members[0] is ForStatement);
            return (ForStatement)tree.Result.Members[0];
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

        private List<Tuple<TokenType, string>> GetForTokens(
            IEnumerable<Tuple<TokenType, string>> initializers,
            IEnumerable<Tuple<TokenType, string>> condition,
            IEnumerable<Tuple<TokenType, string>> iterators,
            IEnumerable<Tuple<TokenType, string>> body)
        {
            return new[]
                {
                    Tuple.Create(TokenType.ForKeyword, "for"),
                    Tuple.Create(TokenType.OpenParenthesis, "(")
                }
                .Concat(initializers)
                .Concat(new[] {Tuple.Create(TokenType.Semicolon, ";")})
                .Concat(condition)
                .Concat(new[] {Tuple.Create(TokenType.Semicolon, ";")})
                .Concat(iterators)
                .Concat(new[] {Tuple.Create(TokenType.CloseParenthesis, ")")})
                .Concat(body)
                .Concat(new[] {Tuple.Create(TokenType.Eof, "\0")})
                .ToList();
        }

        [Test]
        public void SimpleFor()
        {
            var tokens = GetForTokens(
                new[]
                {
                    Tuple.Create(TokenType.Identifier, "i"),
                    Tuple.Create(TokenType.Equals, "="),
                    Tuple.Create(TokenType.Number, "0")
                },
                new[]
                {
                    Tuple.Create(TokenType.Identifier, "i"),
                    Tuple.Create(TokenType.LeftArrow, "<"),
                    Tuple.Create(TokenType.Number, "5")
                },
                new[]
                {
                    Tuple.Create(TokenType.Identifier, "i"),
                    Tuple.Create(TokenType.PlusEquals, "+="),
                    Tuple.Create(TokenType.Number, "1")
                },
                new[]
                {
                    Tuple.Create(TokenType.OpenBrace, "{"),
                    Tuple.Create(TokenType.CloseBrace, "}")
                }
                );
            var stmnt = ForSetUp(GetTokens(tokens));
            Assert.IsTrue(stmnt.Body is BlockStatement);
            Assert.AreEqual(0, ((BlockStatement)stmnt.Body).Statements.Length);
        }

        [Test]
        public void WithoutInitializers()
        {
            var tokens = GetForTokens(
                Array.Empty<Tuple<TokenType, string>>(),
                new[]
                {
                    Tuple.Create(TokenType.Identifier, "i"),
                    Tuple.Create(TokenType.LeftArrow, "<"),
                    Tuple.Create(TokenType.Number, "5")
                },
                new[]
                {
                    Tuple.Create(TokenType.Identifier, "i"),
                    Tuple.Create(TokenType.PlusEquals, "+="),
                    Tuple.Create(TokenType.Number, "1")
                },
                new[]
                {
                    Tuple.Create(TokenType.OpenBrace, "{"),
                    Tuple.Create(TokenType.CloseBrace, "}")
                }
                );
            var stmnt = ForSetUp(GetTokens(tokens));
            Assert.AreEqual(0, stmnt.Initializers.Length);
        }

        [Test]
        public void WithoutIterators()
        {
            var tokens = GetForTokens(
                new[]
                {
                    Tuple.Create(TokenType.Identifier, "i"),
                    Tuple.Create(TokenType.Equals, "="),
                    Tuple.Create(TokenType.Number, "0")
                },
                new[]
                {
                    Tuple.Create(TokenType.Identifier, "i"),
                    Tuple.Create(TokenType.LeftArrow, "<"),
                    Tuple.Create(TokenType.Number, "5")
                },
                Array.Empty<Tuple<TokenType, string>>(),
                new[]
                {
                    Tuple.Create(TokenType.OpenBrace, "{"),
                    Tuple.Create(TokenType.CloseBrace, "}")
                }
            );
            var stmnt = ForSetUp(GetTokens(tokens));
            Assert.AreEqual(0, stmnt.Iterators.Length);
        }

        [Test]
        public void BinaryInitCondIter()
        {
            var tokens = GetForTokens(
                new[]
                {
                    Tuple.Create(TokenType.Identifier, "i"),
                    Tuple.Create(TokenType.LeftArrow, "<"),
                    Tuple.Create(TokenType.Number, "0")
                },
                new[]
                {
                    Tuple.Create(TokenType.Identifier, "i"),
                    Tuple.Create(TokenType.LeftArrow, "<"),
                    Tuple.Create(TokenType.Number, "5")
                },
                new[]
                {
                    Tuple.Create(TokenType.Identifier, "i"),
                    Tuple.Create(TokenType.RightArrow, ">"),
                    Tuple.Create(TokenType.Number, "1")
                },
                new[]
                {
                    Tuple.Create(TokenType.OpenBrace, "{"),
                    Tuple.Create(TokenType.CloseBrace, "}")
                }
            );
            var stmnt = ForSetUp(GetTokens(tokens));
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
                    Tuple.Create(TokenType.Identifier, "i"),
                    Tuple.Create(TokenType.Equals, "="),
                    Tuple.Create(TokenType.Number, "0"),
                    Tuple.Create(TokenType.Comma, ","),
                    Tuple.Create(TokenType.Identifier, "j"),
                    Tuple.Create(TokenType.Equals, "="),
                    Tuple.Create(TokenType.Number, "0"),
                    Tuple.Create(TokenType.Comma, ","),
                    Tuple.Create(TokenType.Identifier, "k"),
                    Tuple.Create(TokenType.Equals, "="),
                    Tuple.Create(TokenType.Number, "0"),
                },
                new[]
                {
                    Tuple.Create(TokenType.Identifier, "i"),
                    Tuple.Create(TokenType.LeftArrow, "<"),
                    Tuple.Create(TokenType.Number, "5")
                },
                new[]
                {
                    Tuple.Create(TokenType.Identifier, "i"),
                    Tuple.Create(TokenType.PlusEquals, "+="),
                    Tuple.Create(TokenType.Number, "1")
                },
                new[]
                {
                    Tuple.Create(TokenType.OpenBrace, "{"),
                    Tuple.Create(TokenType.CloseBrace, "}")
                }
            );
            var stmnt = ForSetUp(GetTokens(tokens));
            Assert.AreEqual(3, stmnt.Initializers.Length);
        }

        [Test]
        public void MultipleIter()
        {
            var tokens = GetForTokens(
                new[]
                {
                    Tuple.Create(TokenType.Identifier, "i"),
                    Tuple.Create(TokenType.Equals, "="),
                    Tuple.Create(TokenType.Number, "0")
                },
                new[]
                {
                    Tuple.Create(TokenType.Identifier, "i"),
                    Tuple.Create(TokenType.LeftArrow, "<"),
                    Tuple.Create(TokenType.Number, "5")
                },
                new[]
                {
                    Tuple.Create(TokenType.Identifier, "i"),
                    Tuple.Create(TokenType.PlusEquals, "+="),
                    Tuple.Create(TokenType.Number, "1"),
                    Tuple.Create(TokenType.Comma, ","),
                    
                    Tuple.Create(TokenType.Identifier, "j"),
                    Tuple.Create(TokenType.PlusEquals, "+="),
                    Tuple.Create(TokenType.Number, "1"),
                    Tuple.Create(TokenType.Comma, ","),
                    
                    Tuple.Create(TokenType.Identifier, "k"),
                    Tuple.Create(TokenType.PlusEquals, "+="),
                    Tuple.Create(TokenType.Number, "1"),
                    Tuple.Create(TokenType.Comma, ","),
                    
                    Tuple.Create(TokenType.Identifier, "l"),
                    Tuple.Create(TokenType.PlusEquals, "+="),
                    Tuple.Create(TokenType.Number, "1"),
                },
                new[]
                {
                    Tuple.Create(TokenType.OpenBrace, "{"),
                    Tuple.Create(TokenType.CloseBrace, "}")
                }
            );
            var stmnt = ForSetUp(GetTokens(tokens));
            Assert.AreEqual(4, stmnt.Iterators.Length);
        }

        [Test]
        public void BooleanCondition()
        {
            var tokens = GetForTokens(
                new[]
                {
                    Tuple.Create(TokenType.Identifier, "i"),
                    Tuple.Create(TokenType.Equals, "="),
                    Tuple.Create(TokenType.Number, "0")
                },
                new[]
                {
                    Tuple.Create(TokenType.TrueKeyword, "true"),
                },
                new[]
                {
                    Tuple.Create(TokenType.Identifier, "i"),
                    Tuple.Create(TokenType.PlusEquals, "+="),
                    Tuple.Create(TokenType.Number, "1")
                },
                new[]
                {
                    Tuple.Create(TokenType.OpenBrace, "{"),
                    Tuple.Create(TokenType.CloseBrace, "}")
                }
            );
            var stmnt = ForSetUp(GetTokens(tokens));
            Assert.IsTrue(stmnt.Condition is BooleanExpression);
        }

        [Test]
        public void VariableAssignmentInitIter()
        {
            var tokens = GetForTokens(
                new[]
                {
                    Tuple.Create(TokenType.Identifier, "i"),
                    Tuple.Create(TokenType.Equals, "="),
                    Tuple.Create(TokenType.Number, "0")
                },
                new[]
                {
                    Tuple.Create(TokenType.Identifier, "i"),
                    Tuple.Create(TokenType.LeftArrow, "<"),
                    Tuple.Create(TokenType.Number, "5")
                },
                new[]
                {
                    Tuple.Create(TokenType.Identifier, "i"),
                    Tuple.Create(TokenType.PlusEquals, "+="),
                    Tuple.Create(TokenType.Number, "1")
                },
                new[]
                {
                    Tuple.Create(TokenType.OpenBrace, "{"),
                    Tuple.Create(TokenType.CloseBrace, "}")
                }
            );
            var stmnt = ForSetUp(GetTokens(tokens));
            Assert.AreEqual(1, stmnt.Initializers.Length);
            Assert.IsTrue(stmnt.Initializers[0] is VariableAssignmentExpression);
            Assert.AreEqual(1, stmnt.Iterators.Length);
            Assert.IsTrue(stmnt.Iterators[0] is VariableAssignmentExpression);
        }

        [Test]
        public void BodyStatement()
        {
            var tokens = GetForTokens(
                Array.Empty<Tuple<TokenType, string>>(),
                new[]
                {
                    Tuple.Create(TokenType.TrueKeyword, "true"),
                },
                Array.Empty<Tuple<TokenType, string>>(),
                new[]
                {
                    Tuple.Create(TokenType.OpenBrace, "{"),
                    Tuple.Create(TokenType.BreakKeyword, "break"),
                    Tuple.Create(TokenType.Semicolon, ";"),
                    Tuple.Create(TokenType.CloseBrace, "}")
                }
            );
            var stmnt = ForSetUp(GetTokens(tokens));
            Assert.IsTrue(stmnt.Body is BlockStatement);
            Assert.AreEqual(1, ((BlockStatement)stmnt.Body).Statements.Length);
        }
    }
}