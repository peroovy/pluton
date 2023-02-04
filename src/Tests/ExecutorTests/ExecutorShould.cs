using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using Core.Execution;
using Core.Execution.Objects;
using Core.Lexing;
using Core.Syntax.AST;
using Core.Syntax.AST.Expressions;
using Core.Syntax.AST.Expressions.Indexer;
using Core.Syntax.AST.Expressions.Literals;
using Core.Utils.Text;
using Ninject;
using NUnit.Framework;
using Array = Core.Execution.Objects.Array;
using String = Core.Execution.Objects.String;

namespace Tests.ExecutorTests
{
    [TestFixture]
    public class ExecutorShould
    {
        private IExecutor executor;

        [SetUp]
        public void SetUp()
        {
            executor = Core.Interpreter.ConfigureContainer().Get<IExecutor>();
        }

        private string GetText(Expression exp)
        {
            return exp.Location.SourceText.Value.Substring(exp.Span.Start, exp.Span.Length);
        }
        
        private VariableAssignmentExpression AssignVariable(string varName, SourceText stext, Expression value)
        {
            var ident = new SyntaxToken(TokenType.Identifier, varName, new Location(stext, 0, varName.Length));
            var oper = new SyntaxToken(TokenType.Equals, "=", new Location(stext, varName.Length + 1, 1));
            var exp = new VariableAssignmentExpression(ident, oper, value);
            return exp;
        }

        private Obj GetVariable(string v)
        {
            var stext = new SourceText($"{v};");
            var exp = new VariableExpression(new SyntaxToken(TokenType.Identifier, v,
                new Location(stext, 0, v.Length)));
            return exp.Accept(executor);
        }

        private Obj CreateArray(string v, params int[] items)
        {
            var stext = new SourceText($"{v} = [{string.Join(", ", items)}];");
            var itemsExp = new List<Expression>();
            var pos = v.Length + 4;
            foreach (var item in items)
            {
                itemsExp.Add(new NumberExpression(new SyntaxToken(TokenType.Number, item.ToString(), 
                    new Location(stext, pos, item.ToString().Length))));
                pos += 2;
            }

            pos -= 2;
            var aExp = new ArrayExpression(
                new SyntaxToken(TokenType.OpenBracket, "[", new Location(stext, v.Length + 3, 1)),
                itemsExp.ToImmutableArray(),
                new SyntaxToken(TokenType.CloseBracket, "]", 
                    new Location(stext, pos, 1))
            );
            var assignment = AssignVariable(v, stext, aExp);
            return assignment.Accept(executor);
        }
        
        [TestCase("a", 1, 33, 7)]
        [TestCase("test", 1)]
        [TestCase("empty")]
        public void AssignArrayToVariable(string v, params int[] items)
        {
            CreateArray(v, items);
            var result = GetVariable(v);
            Assert.IsTrue(result is Array);
            var arrRes = (Array)result;
            for (var i = 0; i < items.Length; i++)
            {
                var value = arrRes[i];
                Assert.IsTrue(value is Number);
                var num = (Number)value;
                Assert.IsTrue(num.IsInteger);
                Assert.AreEqual(items[i], num.AsInteger);
            }
        }

        [TestCase("trueVariable", true, TokenType.TrueKeyword)]
        [TestCase("falseVariable", false, TokenType.FalseKeyword)]
        public void AssignBoolToVariable(string v, bool value, TokenType keyword)
        {
            var stext = new SourceText($"{v} = {value.ToString()};");
            var bExp = new BooleanExpression(new SyntaxToken(keyword, value.ToString(),
                new Location(stext, v.Length + 3, value.ToString().Length)));
            var assignment = AssignVariable(v, stext, bExp);
            assignment.Accept(executor);
            var result = GetVariable(v);
            Assert.IsTrue(result is Bool);
            Assert.AreEqual(value, ((Bool)result).Value);
        }

        [Test]
        public void AssignNullToVariable()
        {
            const string v = "nullVariable";
            var stext = new SourceText($"{v} = null;");
            var nExp = new NullExpression(new SyntaxToken(TokenType.NullKeyword, "null",
                new Location(stext, v.Length + 3, 4)));
            var assignment = AssignVariable(v, stext, nExp);
            assignment.Accept(executor);
            var result = GetVariable(v);
            Assert.IsTrue(result is Null);
        }

        [TestCase("test", 1337)]
        [TestCase("doubleTest", 1.1)]
        [TestCase("zeroTest", 0)]
        public void AssignNumToVariable(string v, double value)
        {
            var stext = new SourceText($"{v} = {value.ToString(CultureInfo.InvariantCulture)};");
            var nExp = new NumberExpression(new SyntaxToken(TokenType.Number, value.ToString(CultureInfo.InvariantCulture),
                new Location(stext, v.Length + 3, value.ToString(CultureInfo.InvariantCulture).Length)));
            var assignment = AssignVariable(v, stext, nExp);
            assignment.Accept(executor);
            var result = GetVariable(v);
            Assert.IsTrue(result is Number);
            Assert.AreEqual(value, ((Number)result).Value);
        }

        [TestCase("numStr", "1337")]
        [TestCase("test", "test")]
        [TestCase("emptyTest", "")]
        [TestCase("keyword", "true")]
        public void AssignStringToVariable(string v, string value)
        {
            var stext = new SourceText($"{v} = \"{value}\";");
            var nExp = new StringExpression(new SyntaxToken(TokenType.Number, value,
                new Location(stext, v.Length + 3, value.Length + 2)));
            var assignment = AssignVariable(v, stext, nExp);
            assignment.Accept(executor);
            var result = GetVariable(v);
            Assert.IsTrue(result is String);
            Assert.AreEqual(value, ((String)result).Value);
        }

        [TestCase(0, "test")]
        [TestCase(1, "test2")]
        [TestCase(2, "1")]
        public void IndexAssignment(int idx, string newValue)
        {
            const string v = "a";
            CreateArray(v, 1, 2, 3);

            var sIdx = idx.ToString();
            var stext = new SourceText($"{v}[{idx}] = \"{newValue}\";");
            var indexed = new VariableExpression(
                new SyntaxToken(TokenType.Identifier, v, new Location(stext, 0, v.Length)));

            var index = new Index(
                new SyntaxToken(TokenType.OpenBracket, "[", new Location(stext, v.Length, 1)),
                new NumberExpression(new SyntaxToken(TokenType.Number, sIdx, new Location(stext, v.Length + 1, sIdx.Length))),
                new SyntaxToken(TokenType.CloseBracket, "]", new Location(stext, v.Length + sIdx.Length + 1, 1))
                );

            var assignment = new IndexAssignmentExpression(
                indexed,
                index,
                new SyntaxToken(TokenType.Equals, "=", new Location(stext, v.Length + sIdx.Length + 2, 1)),
                new StringExpression(
                    new SyntaxToken(TokenType.String, newValue,
                        new Location(stext, v.Length + sIdx.Length + 5, newValue.Length + 2))));
            var result = assignment.Accept(executor);
            Assert.IsTrue(result is String);
            var vValue = GetVariable(v);
            Assert.IsTrue(vValue is Array);
            var arr = (Array)vValue;
            Assert.IsTrue(arr[idx] is String);
            Assert.AreEqual(newValue, ((String)arr[idx]).Value);
        }

        [TestCase(0, 1, 2, 3)]
        [TestCase(1, 4, 343, 2212, 356, 232, 4545 ,232)]
        [TestCase(5, 1, 2, 3, 4, 5, 1337)]
        public void IndexAccess(int idx, params int[] items)
        {
            const string v = "b";
            CreateArray(v, items);

            var stext = new SourceText($"{v}[{idx}];");
            var accessExp = new IndexAccessExpression(
                new VariableExpression(new SyntaxToken(TokenType.Identifier, v, new Location(stext, 0, v.Length))),
                new Index(
                    new SyntaxToken(TokenType.OpenBracket, "[", new Location(stext, v.Length, 1)),
                    new NumberExpression(new SyntaxToken(TokenType.Number, idx.ToString(), 
                        new Location(stext, v.Length + 1, idx.ToString().Length))),
                    new SyntaxToken(TokenType.CloseBracket, "]", new Location(stext, v.Length + idx.ToString().Length + 1, 1))
                    )
                );
            var result = accessExp.Accept(executor);
            Assert.IsTrue(result is Number);
            var num = (Number)result;
            Assert.IsTrue(num.IsInteger);
            Assert.AreEqual(items[idx], num.AsInteger);
        }

        [Test]
        public void FunctionDeclaration()
        {
            const string f = "f";
            var declStext = new SourceText($"def {f}() " + "{}");
            var decl = new FunctionDeclarationStatement(
                new SyntaxToken(TokenType.DefKeyword, "def", new Location(declStext, 0, 3)),
                new SyntaxToken(TokenType.Identifier, f, new Location(declStext, 4, f.Length)),
                new SyntaxToken(TokenType.OpenParenthesis, "(", new Location(declStext, f.Length + 4, 1)),
                ImmutableArray<SyntaxToken>.Empty, 
                ImmutableArray<DefaultParameter>.Empty, 
                new SyntaxToken(TokenType.CloseParenthesis, ")", new Location(declStext, f.Length + 5, 1)),
                new BlockStatement(
                    new SyntaxToken(TokenType.OpenBrace, "{", new Location(declStext, f.Length + 7, 1)),
                    ImmutableArray<Statement>.Empty, 
                    new SyntaxToken(TokenType.CloseBrace, "}", new Location(declStext, f.Length + 8, 1))
                    )
                );
            decl.Accept(executor);

            var fStext = new SourceText($"{f};");
            var vExp = new VariableExpression(
                new SyntaxToken(TokenType.Identifier, f, new Location(fStext, 0, f.Length))
                );
            var result = vExp.Accept(executor);
            Assert.IsTrue(result is Function);
        }

        [TestCase(1, 5, 2)]
        [TestCase(100, 3, 1)]
        [TestCase(1, 3, 37)]
        public void ForShould(double startValue, int iterations, double multiplier)
        {
            const string v = "b";
            AssignNumToVariable(v, startValue);

            var sIter = iterations.ToString();
            var sMult = multiplier.ToString(CultureInfo.InvariantCulture);
            var forStext = new SourceText("for (i = 0; i < " + sIter + "; i += 1) {" + v + " *= " + sMult + ";}");
            var forStatement = new ForStatement(
                new SyntaxToken(TokenType.ForKeyword, "for", new Location(forStext, 0, 3)),
                new SyntaxToken(TokenType.OpenParenthesis, "(", new Location(forStext, 4, 1)),
                new[]
                    {
                        (Expression)new VariableAssignmentExpression(
                            new SyntaxToken(TokenType.Identifier, "i", new Location(forStext, 5, 1)),
                            new SyntaxToken(TokenType.Equals, "=", new Location(forStext, 7, 1)),
                            new NumberExpression(new SyntaxToken(TokenType.Number, "0", new Location(forStext, 9, 1)))
                            )    
                    }.ToImmutableArray(),
                new SyntaxToken(TokenType.Semicolon, ";", new Location(forStext, 10, 1)),
                new BinaryExpression(
                    new VariableExpression(new SyntaxToken(TokenType.Identifier, "i", new Location(forStext, 12, 1))),
                    new SyntaxToken(TokenType.LeftArrow, "<", new Location(forStext, 14, 1)),
                    new NumberExpression(new SyntaxToken(TokenType.Number, sIter, 
                        new Location(forStext, 16, sIter.Length)))
                    ),
                new SyntaxToken(TokenType.Semicolon, ";", new Location(forStext, sIter.Length + 16, 1)),
                new []
                {
                    (Expression)new VariableAssignmentExpression(
                        new SyntaxToken(TokenType.Identifier, "i", new Location(forStext, sIter.Length + 18, 1)),
                        new SyntaxToken(TokenType.PlusEquals, "+=", new Location(forStext, sIter.Length + 20, 2)),
                        new BinaryExpression(
                            new VariableExpression(new SyntaxToken(TokenType.Identifier, "i", new Location(forStext, sIter.Length + 18, 1))),
                            new SyntaxToken(TokenType.Plus, "+", new Location(forStext, sIter.Length + 20, 1)),
                            new NumberExpression(new SyntaxToken(TokenType.Number, "1", new Location(forStext, sIter.Length + 23, 1)))
                            )
                        )
                }.ToImmutableArray(),
                new SyntaxToken(TokenType.CloseParenthesis, ")", new Location(forStext, sIter.Length + 24, 1)),
                new BlockStatement(
                    new SyntaxToken(TokenType.OpenBrace, "{", new Location(forStext, sIter.Length + 26, 1)),
                    new []
                    {
                        (Statement)new ExpressionStatement(
                            new VariableAssignmentExpression(
                                new SyntaxToken(TokenType.Identifier, v, new Location(forStext, sIter.Length + 27, v.Length)),
                                new SyntaxToken(TokenType.StarEquals, "*=", 
                                    new Location(forStext, sIter.Length + v.Length + 28, 2)),
                                new BinaryExpression(
                                    new VariableExpression(new SyntaxToken(TokenType.Identifier, v, 
                                        new Location(forStext, sIter.Length + 27, v.Length))),
                                    new SyntaxToken(TokenType.Star, "*", 
                                        new Location(forStext, sIter.Length + v.Length + 28, 1)),
                                    new NumberExpression(new SyntaxToken(TokenType.Number, sMult, 
                                        new Location(forStext, sIter.Length + v.Length + 31, sMult.Length)))
                                    )
                                ),
                            new SyntaxToken(TokenType.Semicolon, ";", 
                                new Location(forStext, sIter.Length + v.Length + sMult.Length + 31, 1))
                            )
                    }.ToImmutableArray(),
                    new SyntaxToken(TokenType.CloseBrace, "}", new Location(forStext, sIter.Length + v.Length + sMult.Length + 32, 1))
                    )
            );
            forStatement.Accept(executor);
            var result = GetVariable(v);
            Assert.IsTrue(result is Number);
            var actual = ((Number)result).Value;
            var expected = startValue;
            for (var i = 0; i < iterations; i++)
            {
                expected *= multiplier;
            }
            Assert.AreEqual(expected, actual);
        }

        [TestCase(1, 5, 0, 1, 7)]
        [TestCase(100, -1, 0, 0.1, 7)]
        [TestCase(0, 100000, 100, 15, 1)]
        [TestCase(0, 1, 3, 3, 7)]
        public void WhileShould(double startValue, double plusValue, double startIter, double plusIter, double limit)
        {
            const string v = "test";
            var pvs = plusValue.ToString(CultureInfo.InvariantCulture);
            var pis = plusIter.ToString(CultureInfo.InvariantCulture);
            var ls = limit.ToString(CultureInfo.InvariantCulture);
            
            AssignNumToVariable(v, startValue);
            AssignNumToVariable("i", startIter);

            var wStext = new SourceText($"while ({ls} > i) " + "{ " + $"{v} = i + {pvs}; i += {pis};" + " }");

            var whileStatement = new WhileStatement(
                new SyntaxToken(TokenType.WhileKeyword, "while", new Location(wStext, 0, 5)),
                new SyntaxToken(TokenType.OpenParenthesis, "(", new Location(wStext, 6, 1)),
                new BinaryExpression(
                    new NumberExpression(new SyntaxToken(TokenType.Number, ls, new Location(wStext, 7, ls.Length))),
                    new SyntaxToken(TokenType.RightArrow, ">", new Location(wStext, ls.Length + 8, 1)),
                    new VariableExpression(new SyntaxToken(TokenType.Identifier, "i", new Location(wStext, ls.Length + 10, 1)))
                    ),
                new SyntaxToken(TokenType.CloseParenthesis, ")", new Location(wStext, ls.Length + 11, 1)),
                new BlockStatement(
                    new SyntaxToken(TokenType.OpenBrace, "{", new Location(wStext, ls.Length + 13, 1)),
                    new[]
                    {
                        (Statement)new ExpressionStatement(
                            new VariableAssignmentExpression(
                                new SyntaxToken(TokenType.Identifier, v, 
                                    new Location(wStext, ls.Length + 15, v.Length)),
                                new SyntaxToken(TokenType.Equals, "=", 
                                    new Location(wStext, ls.Length + v.Length + 16, 1)),
                                new BinaryExpression(
                                    new VariableExpression(new SyntaxToken(TokenType.Identifier, "i", 
                                        new Location(wStext, ls.Length + v.Length + 18, 1))),
                                    new SyntaxToken(TokenType.Plus, "+", 
                                        new Location(wStext, ls.Length + v.Length + 20, 1)),
                                    new NumberExpression(new SyntaxToken(TokenType.Number, pvs, 
                                        new Location(wStext, ls.Length + v.Length + 22, pvs.Length)))
                                    )
                                ),
                            new SyntaxToken(TokenType.Semicolon, ";", 
                                new Location(wStext, ls.Length + v.Length + pvs.Length + 22, 1))
                            ),
                        new ExpressionStatement(
                            new VariableAssignmentExpression(
                                new SyntaxToken(TokenType.Identifier, "i", 
                                    new Location(wStext, ls.Length + v.Length + pvs.Length + 24, 1)),
                                new SyntaxToken(TokenType.PlusEquals, "+=", 
                                    new Location(wStext, ls.Length + v.Length + pvs.Length + 26, 2)),
                                new BinaryExpression(
                                    new VariableExpression(new SyntaxToken(TokenType.Identifier, "i",
                                        new Location(wStext, ls.Length + v.Length + pvs.Length + 24, 1))),
                                    new SyntaxToken(TokenType.Plus, "+", 
                                        new Location(wStext, ls.Length + v.Length + pvs.Length + 26, 1)),
                                    new NumberExpression(new SyntaxToken(TokenType.Number, pis,
                                        new Location(wStext, ls.Length + v.Length + pvs.Length + 29, pis.Length)))
                                    )
                                ),
                            new SyntaxToken(TokenType.Semicolon, ";", 
                                new Location(wStext, ls.Length + v.Length + pvs.Length + pis.Length + 29, 1))
                            )
                    }.ToImmutableArray(),
                    new SyntaxToken(TokenType.CloseBrace, "}", 
                        new Location(wStext, ls.Length + v.Length + pvs.Length + pis.Length + 31, 1))
                    )
                );
            whileStatement.Accept(executor);
            var expected = startValue;
            var i = startIter;
            while (limit > i)
            {
                expected = i + plusValue;
                i += plusIter;
            }

            var result = GetVariable(v);
            Assert.IsTrue(result is Number);
            Assert.AreEqual(expected, ((Number)result).Value);
        }

        [TestCase(true, TokenType.TrueKeyword, "test passed", "test not passed")]
        public void IfElseShould(bool condition, TokenType conditionType, string ifValue, string elseValue)
        {
            const string v = "test";
            AssignStringToVariable(v, "none");

            var ifStext = new SourceText($"if ({condition.ToString()}) {v} = \"{ifValue}\"; else {v} = \"{elseValue}\";");
            var ifStatement = new IfStatement(
                new SyntaxToken(TokenType.IfKeyword, "if", new Location(ifStext, 0, 2)),
                new SyntaxToken(TokenType.OpenParenthesis, "(", new Location(ifStext, 3, 1)),
                new BooleanExpression(new SyntaxToken(conditionType, condition.ToString(), 
                    new Location(ifStext, 4, condition.ToString().Length))),
                new SyntaxToken(TokenType.CloseParenthesis, ")", new Location(ifStext, condition.ToString().Length + 4, 1)),
                new ExpressionStatement(
                    new VariableAssignmentExpression(
                        new SyntaxToken(TokenType.Identifier, v, new Location(ifStext, condition.ToString().Length + 6, v.Length)),
                        new SyntaxToken(TokenType.Equals, "=", new Location(ifStext, condition.ToString().Length + v.Length + 7, 1)),
                        new StringExpression(new SyntaxToken(TokenType.String, ifValue, 
                            new Location(ifStext, condition.ToString().Length + v.Length + 9, ifValue.Length + 2)))
                        ),
                    new SyntaxToken(TokenType.Semicolon, ";", 
                        new Location(ifStext, condition.ToString().Length + v.Length + ifValue.Length + 11, 1))
                    ),
                new ElseClause(
                    new SyntaxToken(TokenType.ElseKeyword, "else", 
                        new Location(ifStext, condition.ToString().Length + v.Length + ifValue.Length + 13, 4)),
                    new ExpressionStatement(
                        new VariableAssignmentExpression(
                            new SyntaxToken(TokenType.Identifier, v, 
                                new Location(ifStext, condition.ToString().Length + v.Length + ifValue.Length + 18, v.Length)),
                            new SyntaxToken(TokenType.Equals, "=", 
                                new Location(ifStext, condition.ToString().Length + v.Length + ifValue.Length + v.Length + 19, 1)),
                            new StringExpression(new SyntaxToken(TokenType.String, elseValue, 
                                new Location(ifStext, condition.ToString().Length + v.Length + ifValue.Length + v.Length + 21, elseValue.Length)))
                            ),
                        new SyntaxToken(TokenType.Semicolon, ";", 
                            new Location(ifStext, condition.ToString().Length + v.Length + ifValue.Length + v.Length + elseValue.Length + 21, 1))
                        )
                    )
            );
            ifStatement.Accept(executor);
            var result = GetVariable(v);
            Assert.IsTrue(result is String);
            if (condition)
                Assert.AreEqual(ifValue, ((String)result).Value);
            else
                Assert.AreEqual(elseValue, ((String)result).Value);
        }

        [TestCase("test", 123)]
        [TestCase("leet", 1337)]
        [TestCase("zero", 0)]
        public void VisibleFromParentScope(string v, double value)
        {
            var sv = value.ToString(CultureInfo.InvariantCulture);
            var stext = new SourceText("{" + $"{v} = {sv};" + "{" + $"{v};" + "}" + "}");
            var blockStatement = new BlockStatement(
                new SyntaxToken(TokenType.OpenBrace, "{", new Location(stext, 0, 1)),
                new[]
                {
                    (Statement)new ExpressionStatement(
                        new VariableAssignmentExpression(
                            new SyntaxToken(TokenType.Identifier, v, new Location(stext, 1, v.Length)),
                            new SyntaxToken(TokenType.Equals, "=", new Location(stext, v.Length + 2, 1)),
                            new NumberExpression(new SyntaxToken(TokenType.Number, sv, new Location(stext, v.Length + 4, sv.Length)))
                            ),
                        new SyntaxToken(TokenType.Semicolon, ";", new Location(stext, v.Length + sv.Length + 4, 1))
                        ),
                    new BlockStatement(
                        new SyntaxToken(TokenType.OpenBrace, "{", new Location(stext, v.Length + sv.Length + 5, 1)),
                        new[]
                        {
                            (Statement)new ExpressionStatement(
                                new VariableExpression(new SyntaxToken(TokenType.Identifier, v, 
                                    new Location(stext, v.Length + sv.Length + 6, v.Length))),
                                new SyntaxToken(TokenType.Semicolon, ";", 
                                    new Location(stext, v.Length * 2 + sv.Length + 6, 1))
                                )
                        }.ToImmutableArray(),
                        new SyntaxToken(TokenType.CloseBrace, "}",
                            new Location(stext, v.Length * 2 + sv.Length + 7, 1))
                        )
                }.ToImmutableArray(),
                new SyntaxToken(TokenType.CloseBrace, "}",
                    new Location(stext, v.Length * 2 + sv.Length + 8, 1))
                );
            Assert.DoesNotThrow(() => blockStatement.Accept(executor));
        }

        [TestCase("test", 123)]
        [TestCase("leet", 1337)]
        [TestCase("zero", 0)]
        public void NotVisibleFromChildScope(string v, double value)
        {
            var sv = value.ToString(CultureInfo.InvariantCulture);
            var stext = new SourceText("{{" + $"{v} = {sv};" + "}" + $"{v};" + "}");
            var blockStatement = new BlockStatement(
                new SyntaxToken(TokenType.OpenBrace, "{", new Location(stext, 0, 1)),
                new[]
                {
                    (Statement)new BlockStatement(
                        new SyntaxToken(TokenType.OpenBrace, "{", new Location(stext, v.Length + sv.Length + 1, 1)),
                        new[]
                        {
                            (Statement)new ExpressionStatement(
                                new VariableAssignmentExpression(
                                    new SyntaxToken(TokenType.Identifier, v, new Location(stext, 2, v.Length)),
                                    new SyntaxToken(TokenType.Equals, "=", new Location(stext, v.Length + 3, 1)),
                                    new NumberExpression(new SyntaxToken(TokenType.Number, sv, new Location(stext, v.Length + 5, sv.Length)))
                                ),
                                new SyntaxToken(TokenType.Semicolon, ";", new Location(stext, v.Length + sv.Length + 5, 1))
                            ),
                        }.ToImmutableArray(),
                        new SyntaxToken(TokenType.CloseBrace, "}",
                            new Location(stext, v.Length + sv.Length + 6, 1))
                        ),
                    new ExpressionStatement(
                        new VariableExpression(new SyntaxToken(TokenType.Identifier, v, 
                            new Location(stext, v.Length + sv.Length + 7, v.Length))),
                        new SyntaxToken(TokenType.Semicolon, ";", 
                            new Location(stext, v.Length * 2 + sv.Length + 7, 1))
                    ),
                }.ToImmutableArray(),
                new SyntaxToken(TokenType.CloseBrace, "}",
                    new Location(stext, v.Length * 2 + sv.Length + 8, 1))
                );
            Assert.Throws<RuntimeException>(() => blockStatement.Accept(executor));
        }

        [TestCase("test", 123)]
        [TestCase("leet", 1337)]
        [TestCase("zero", 0)]
        public void NotVisibleFromOtherScope(string v, double value)
        {
            var sv = value.ToString(CultureInfo.InvariantCulture);
            var stext = new SourceText("{{" + $"{v} = {sv};" + "}{" + $"{v};" + "}}");
            var blockStatement = new BlockStatement(
                new SyntaxToken(TokenType.OpenBrace, "{", new Location(stext, 0, 1)),
                new[]
                {
                    (Statement)new BlockStatement(
                        new SyntaxToken(TokenType.OpenBrace, "{", new Location(stext, v.Length + sv.Length + 1, 1)),
                        new[]
                        {
                            (Statement)new ExpressionStatement(
                                new VariableAssignmentExpression(
                                    new SyntaxToken(TokenType.Identifier, v, new Location(stext, 2, v.Length)),
                                    new SyntaxToken(TokenType.Equals, "=", new Location(stext, v.Length + 3, 1)),
                                    new NumberExpression(new SyntaxToken(TokenType.Number, sv, new Location(stext, v.Length + 5, sv.Length)))
                                ),
                                new SyntaxToken(TokenType.Semicolon, ";", new Location(stext, v.Length + sv.Length + 5, 1))
                            ),
                        }.ToImmutableArray(),
                        new SyntaxToken(TokenType.CloseBrace, "}",
                            new Location(stext, v.Length + sv.Length + 6, 1))
                        ),
                    (Statement)new BlockStatement(
                        new SyntaxToken(TokenType.OpenBrace, "{", new Location(stext, v.Length + sv.Length + 7, 1)),
                        new[]
                        {
                            (Statement)new ExpressionStatement(
                                new VariableExpression(new SyntaxToken(TokenType.Identifier, v, 
                                    new Location(stext, v.Length + sv.Length + 8, v.Length))),
                                new SyntaxToken(TokenType.Semicolon, ";", 
                                    new Location(stext, v.Length * 2 + sv.Length + 8, 1))
                            ),
                        }.ToImmutableArray(),
                        new SyntaxToken(TokenType.CloseBrace, "}",
                            new Location(stext, v.Length * 2 + sv.Length + 9, 1))
                        ),
                }.ToImmutableArray(),
                new SyntaxToken(TokenType.CloseBrace, "}",
                    new Location(stext, v.Length * 2 + sv.Length + 10, 1))
                );
            Assert.Throws<RuntimeException>(() => blockStatement.Accept(executor));
        }

        [TestCase(2, 2)]
        [TestCase(1, 337)]
        [TestCase(0, 1)]
        public void BinaryPlus(int v1, int v2)
        {
            var s1 = v1.ToString();
            var s2 = v2.ToString();
            var stext = new SourceText($"{s1} + {s2}");
            var b = new BinaryExpression(
                new NumberExpression(new SyntaxToken(TokenType.Number, s1, new Location(stext, 0, s1.Length))),
                new SyntaxToken(TokenType.Plus, "+", new Location(stext, s1.Length + 1, 1)),
                new NumberExpression(new SyntaxToken(TokenType.Number, s2, new Location(stext, s1.Length + 3, s2.Length)))
            );
            var result = b.Accept(executor);
            Assert.IsTrue(result is Number);
            Assert.AreEqual(v1 + v2, ((Number)result).Value);
        }

        [TestCase(2, 2)]
        [TestCase(1, 337)]
        [TestCase(0, 1)]
        public void BinaryMinus(int v1, int v2)
        {
            var s1 = v1.ToString();
            var s2 = v2.ToString();
            var stext = new SourceText($"{s1} - {s2}");
            var b = new BinaryExpression(
                new NumberExpression(new SyntaxToken(TokenType.Number, s1, new Location(stext, 0, s1.Length))),
                new SyntaxToken(TokenType.Minus, "-", new Location(stext, s1.Length + 1, 1)),
                new NumberExpression(new SyntaxToken(TokenType.Number, s2, new Location(stext, s1.Length + 3, s2.Length)))
            );
            var result = b.Accept(executor);
            Assert.IsTrue(result is Number);
            Assert.AreEqual(v1 - v2, ((Number)result).Value);
        }

        [TestCase(2, 2)]
        [TestCase(1, 337)]
        [TestCase(0, 1)]
        public void BinaryStar(int v1, int v2)
        {
            var s1 = v1.ToString();
            var s2 = v2.ToString();
            var stext = new SourceText($"{s1} * {s2}");
            var b = new BinaryExpression(
                new NumberExpression(new SyntaxToken(TokenType.Number, s1, new Location(stext, 0, s1.Length))),
                new SyntaxToken(TokenType.Star, "*", new Location(stext, s1.Length + 1, 1)),
                new NumberExpression(new SyntaxToken(TokenType.Number, s2, new Location(stext, s1.Length + 3, s2.Length)))
            );
            var result = b.Accept(executor);
            Assert.IsTrue(result is Number);
            Assert.AreEqual(v1 * v2, ((Number)result).Value);
        }

        [TestCase(2, 2)]
        [TestCase(1, 337)]
        [TestCase(10, 2)]
        public void BinarySlash(int v1, int v2)
        {
            var s1 = v1.ToString();
            var s2 = v2.ToString();
            var stext = new SourceText($"{s1} * {s2}");
            var b = new BinaryExpression(
                new NumberExpression(new SyntaxToken(TokenType.Number, s1, new Location(stext, 0, s1.Length))),
                new SyntaxToken(TokenType.Slash, "/", new Location(stext, s1.Length + 1, 1)),
                new NumberExpression(new SyntaxToken(TokenType.Number, s2, new Location(stext, s1.Length + 3, s2.Length)))
            );
            var result = b.Accept(executor);
            Assert.IsTrue(result is Number);
            Assert.AreEqual((double)v1 / v2, ((Number)result).Value);
        }
    }
}