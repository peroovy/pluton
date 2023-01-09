using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Core.Lexing;
using Core.Syntax;
using Core.Syntax.AST;
using Core.Utils.Text;
using Ninject;
using NUnit.Framework;

namespace Interpreter.Tests.SyntaxTests
{
    [TestFixture]
    public class ReturnShould
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

        private ReturnStatement ReturnSetUp(ImmutableArray<SyntaxToken> tokens)
        {
            var tree = parser.Parse(
                tokens.Length == 0 ? new SourceText("") : tokens[0].Location.SourceText, tokens);
            Assert.IsFalse(tree.HasErrors);
            Assert.AreEqual(1, tree.Result.Members.Length);
            Assert.IsTrue(tree.Result.Members[0] is ReturnStatement);
            return (ReturnStatement)tree.Result.Members[0];
        }
        
        [Test]
        public void SimpleReturn()
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.ReturnKeyword, "return"),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var r = ReturnSetUp(GetTokens(tokens));
            Assert.IsNull(r.Expression);
        }
        
        [Test]
        public void ReturnStatement()
        {
            var tokens = new[]
            {
                Tuple.Create(TokenType.ReturnKeyword, "return"),
                Tuple.Create(TokenType.Number, "2"),
                Tuple.Create(TokenType.Plus, "+"),
                Tuple.Create(TokenType.Number, "2"),
                Tuple.Create(TokenType.Semicolon, ";"),
                Tuple.Create(TokenType.Eof, "\0"),
            }.ToList();
            var r = ReturnSetUp(GetTokens(tokens));
            Assert.IsNotNull(r.Expression);
        }
    }
}