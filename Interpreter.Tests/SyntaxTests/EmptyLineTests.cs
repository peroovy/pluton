using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Runtime.InteropServices;
using Interpreter.Core.Lexing;
using Interpreter.Core.Syntax;
using Interpreter.Core.Syntax.AST;
using Interpreter.Core.Syntax.AST.Expressions;
using NUnit.Framework;

namespace Interpreter.Tests.SyntaxTests
{
    [TestFixture]
    public class EmptyLineTests
    {
        private ISyntaxParser parser;
        
        [SetUp]
        public void SetUp()
        {
            parser = Core.InterpreterCore.Create().SyntaxParser;
        }

        [Test]
        public void EmptyLine()
        {
            var emptyLine = 
                new[] { new SyntaxToken(TokenTypes.Eof, "\0", new TextLocation()) }
                .ToImmutableArray();
            var tree = parser.Parse(emptyLine);
            Assert.AreEqual(0, tree.Members.Length);
        }
    }
}