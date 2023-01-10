using System;
using Interpreter.Core.Execution;
using Interpreter.Core.Lexing;
using Interpreter.Core.Syntax;
using Interpreter.Core.Syntax.AST;
using Interpreter.Core.Text;
using NUnit.Framework;
using System.Reflection;
using Interpreter.Core.Execution.Objects;

namespace Interpreter.Tests.ExecutionTests
{
    [TestFixture]
    public class ExecutorShould
    {
        private ITextParser textParser;
        private ILexer lexer;
        private ISyntaxParser syntaxParser;
        private Executor executor;
        private Scope scope;

        [SetUp]
        public void SetUp()
        {
            textParser = Core.InterpreterCore.Create().TextParser;
            lexer = Core.InterpreterCore.Create().Lexer;
            syntaxParser = Core.InterpreterCore.Create().SyntaxParser;
            executor = (Executor)Core.InterpreterCore.Create().Executor;
            scope = (Scope)executor.GetType().GetField("scope");
        }

        private SyntaxTree GetTree(string from)
        {
            var lines = textParser.ParseLines(from);
            var tokens = lexer.Tokenize(lines);
            var tree = syntaxParser.Parse(tokens);
            return tree;
        }

        [Test]
        public void VariableAssign()
        {
            var tree = GetTree("a = 5;");
            executor.Execute(tree);
            var actual = (Obj)new Number(0);
            Assert.IsTrue(scope.TryLookup("a", out actual));
            var num = (Number)actual;
            Assert.AreEqual(5, num.Value);
        }
    }
}