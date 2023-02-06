using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using Core.Execution.Objects;
using Core.Execution.Objects.BuiltinFunctions;
using Core.Execution.Objects.DataModel;
using Core.Execution.Operations.Binary;
using Core.Execution.Operations.Unary;
using Core.Execution.Signals;
using Core.Lexing;
using Core.Syntax.AST;
using Core.Syntax.AST.Expressions;
using Core.Syntax.AST.Expressions.Indexer;
using Core.Syntax.AST.Expressions.Literals;
using Core.Utils.Diagnostic;
using Core.Utils.Text;

namespace Core.Execution
{
    public class Executor : IExecutor
    {
        private readonly BinaryOperation[] binaryOperations;
        private readonly UnaryOperation[] unaryOperations;

        private readonly Stack<Function> callStack = new();
        private readonly Stack<LoopStatement> loopStack = new();
        private readonly Scope globalScope = new(null);
        private Scope scope;

        private Obj lastExpressionValue;

        public Executor(
            BinaryOperation[] binaryOperations, 
            UnaryOperation[] unaryOperations, 
            BuiltinFunction[] builtinFunctions)
        {
            this.binaryOperations = binaryOperations;
            this.unaryOperations = unaryOperations;
            
            foreach (var function in builtinFunctions)
                globalScope.Assign(function.Name, function);

            scope = globalScope;
        }

        public TranslationState<Obj> Execute(SyntaxTree tree)
        {
            var diagnosticBag = new DiagnosticBag();
            lastExpressionValue = new Null();
            
            try
            {
                foreach (var member in tree.Members)
                    member.Accept(this);

                return new TranslationState<Obj>(lastExpressionValue, diagnosticBag);
            }
            catch (RuntimeException exception)
            {
                diagnosticBag.AddError(exception.Location, exception.Message);
                
                return new TranslationState<Obj>(null, diagnosticBag);
            }
        }

        public Obj Execute(ClassStatement statement)
        {
            var classScope = new Scope(scope);
            scope = classScope;

            foreach (var member in statement.Members)
                member.Accept(this);

            scope = scope.Parent;

            var obj = new ClassObj(statement.Identifier.Text);
            foreach (var attr in classScope.CurrentLevel)
                obj.SetAttribute(attr.Key, attr.Value);
            
            scope.Assign(obj.Name, obj);

            return null;
        }

        public Obj Execute(FunctionDeclarationStatement statement)
        {
            var positions = statement
                .PositionParameters
                .Select(token => token.Text)
                .ToImmutableArray();

            var defaults = statement
                .DefaultParameters
                .Select(parameter => new CallArgument(parameter.Name.Text, parameter.Expression.Accept(this)))
                .ToImmutableArray();

            var function = new Function(
                statement.Identifier.Text,
                positions,
                defaults,
                context =>
                {
                    try
                    {
                        var enumerator = statement.Block.Statements.GetEnumerator();
                        while (ReferenceEquals(context.Callable, callStack.Peek()) && enumerator.MoveNext())
                            enumerator.Current.Accept(this);
                    }
                    catch (ReturnSignal signal)
                    {
                        return signal.Value;
                    }

                    return new Null();
                }
            );

            scope.Assign(function.Name, function);

            return null;
        }

        public Obj Execute(ReturnStatement statement)
        {
            var value = statement.Expression?.Accept(this) ?? new Null();
            
            throw new ReturnSignal(value);
        }

        public Obj Execute(BreakStatement statement)
        {
            throw new BreakSignal();
        }
        
        public Obj Execute(ContinueStatement statement)
        {
            throw new ContinueSignal();
        }

        public Obj Execute(ForStatement statement)
        {
            ExecuteLoopWithPreCondition(
                statement,
                loopStatement =>
                {
                    foreach (var initializer in loopStatement.Initializers)
                        initializer.Accept(this);
                },
                loopStatement =>
                {
                    foreach (var iterator in loopStatement.Iterators)
                        iterator.Accept(this);
                });

            return null;
        }

        public Obj Execute(WhileStatement statement)
        {
            ExecuteLoopWithPreCondition(statement);
            
            return null;
        }

        public Obj Execute(IfStatement statement)
        {
            var condition = EvaluateCondition(statement.Condition);
            
            if (condition.Value)
            {
                statement.ThenStatement.Accept(this);
            }
            else
            {
                statement.ElseClause?.Accept(this);
            }

            return null;
        }

        public Obj Execute(ElseClause clause)
        {
            clause.Statement.Accept(this);
            
            return null;
        }

        public Obj Execute(BlockStatement block)
        {
            scope = new Scope(scope);
            
            foreach (var statement in block.Statements)
                statement.Accept(this);

            scope = scope.Parent;

            return null;
        }

        public Obj Execute(ExpressionStatement statement)
        {
            lastExpressionValue = statement.Expression.Accept(this);

            return null;
        }

        public Obj Execute(VariableAssignmentExpression assignment)
        {
            var name = assignment.Identifier.Text;
            var value = assignment.Expression.Accept(this);

            if (!TryAssignUp(name, value))
                scope.Assign(name, value);

            return value;
        }

        public Obj Execute(IndexAssignmentExpression assignment)
        {
            var indexedExpression = assignment.IndexedExpression;
            var obj = indexedExpression.Accept(this);

            if (obj is not IIndexSettable settable)
                throw new RuntimeException(indexedExpression.Location, $"Type '{obj.TypeName}' is not settable by index");

            var indexValue = (Number)assignment.Index.Accept(this);
            var value = assignment.Value.Accept(this);

            try
            {
                return settable[indexValue.AsInteger] = value;
            }
            catch (IndexOutOfRangeException)
            {
                var betweenBrackets = GetLocationBetweenBrackets(
                    assignment.Index.OpenBracket, assignment.Index.CloseBracket
                );
                throw new RuntimeException(betweenBrackets, "The index was outside the bounds of the collection");
            }
        }

        public Obj Execute(ParenthesizedExpression expression) => expression.InnerExpression.Accept(this);

        public Obj Execute(IndexAccessExpression expression)
        {
            var indexedExpression = expression.IndexedExpression;
            var obj = indexedExpression.Accept(this);

            if (obj is not IIndexReadable readable)
                throw new RuntimeException(indexedExpression.Location, $"Type '{obj.TypeName}' is not readable by index");

            var indexValue = (Number)expression.Index.Accept(this);
            try
            {
                return readable[indexValue.AsInteger];
            }
            catch (IndexOutOfRangeException)
            {
                var betweenBrackets = GetLocationBetweenBrackets(
                    expression.Index.OpenBracket, expression.Index.CloseBracket
                );
                throw new RuntimeException(betweenBrackets, "The index was outside the bounds of the collection");
            }
        }

        public Obj Execute(Index index)
        {
            var expression = index.Expression;
            var value = expression.Accept(this);
            
            if (value is not Number number)
                throw new RuntimeException(expression.Location, $"Expected number value but was '{value.TypeName}' type");

            if (!number.IsInteger)
                throw new RuntimeException(expression.Location, "Expected integer value");
            
            return number;
        }

        public Obj Execute(BinaryExpression binary)
        {
            var opToken = binary.OperatorToken;
            
            var left = binary.Left.Accept(this);
            var right = binary.Right.Accept(this);
            var function = binaryOperations
                .Single(op => op.IsOperator(opToken.Type))
                .FindOperation(left, right);
            
            if (function is null || function.PositionParameters.Length != 2)
            {
                throw new RuntimeException(
                    opToken.Location,
                    $"The binary operator '{opToken.Text}' is not defined for '{left.TypeName}' and '{right.TypeName}' types"
                );
            }

            var positions = function.PositionParameters;
            var arguments = ImmutableArray.Create(
                new CallArgument(positions[0], left),
                new CallArgument(positions[1], right)
            );

            return InvokeCallableObject(function, arguments);
        }

        public Obj Execute(UnaryExpression unary)
        {
            var opToken = unary.OperatorToken;
            var operand = unary.Operand.Accept(this);
            var function = unaryOperations
                .Single(op => op.IsOperator(opToken.Type))
                .FindMethod(operand);

            if (function is null || function.PositionParameters.Length != 1)
            {
                throw new RuntimeException(
                    opToken.Location,
                    $"The unary operator '{opToken.Text}' is not defined for '{operand.TypeName}' type"
                );
            }

            var arguments = ImmutableArray.Create(new CallArgument(function.PositionParameters[0], operand));

            return InvokeCallableObject(function, arguments);
        }

        public Obj Execute(NumberExpression number)
        {
            var value = Convert.ToDouble(number.Token.Text, CultureInfo.InvariantCulture);

            return new Number(value);
        }

        public Obj Execute(BooleanExpression boolean)
        {
            var value = boolean.Token.Type switch
            {
                TokenType.TrueKeyword => true,
                TokenType.FalseKeyword => false,
                _ => throw new ArgumentException($"Bad boolean token {boolean}")
            };

            return new Bool(value);
        }

        public Obj Execute(StringExpression str) => new Objects.String(str.Token.Text);
        
        public Obj Execute(ArrayExpression array)
        {
            var items = array.Items
                .Select(expression => expression.Accept(this))
                .ToImmutableArray();

            return new Objects.Array(items);
        }

        public Obj Execute(NullExpression expression) => new Null();

        public Obj Execute(VariableExpression variable)
        {
            var identifier = variable.Token;
            
            if (scope.TryLookup(identifier.Text, out var value))
                return value;

            throw new RuntimeException(identifier.Location, $"Variable '{identifier.Text}' does not exist");
        }

        public Obj Execute(CallExpression expression)
        {
            var callableExpression = expression.CallableExpression;
            var obj = callableExpression.Accept(this);

            if (obj is ClassObj classObj)
                obj = classObj.MagicMethodNew;

            if (obj is not Function callable)
                throw new RuntimeException(callableExpression.Location, $"'{obj.TypeName}' object is not callable");
            
            var argumentsCount = expression.Arguments.Length;
            var positionsCount = callable.PositionParameters.Length;
            var defaultsCount = callable.DefaultParameters.Length;

            var offset = 0;
            if (callable is Method method)
            {
                if (positionsCount == 0)
                {
                    throw new RuntimeException(
                        callableExpression.Location,
                        $"Definition of the method '{method.Name}' must contain more than 1 parameter"
                    );
                }

                offset = 1;
                argumentsCount++;
            }
            
            if (argumentsCount < positionsCount || argumentsCount > positionsCount + defaultsCount)
            {
                throw new RuntimeException(
                    callableExpression.Location,
                    $"Callable takes {positionsCount - offset} position arguments but {argumentsCount - offset} were given"
                );
            }
            
            var arguments = EvaluateArguments(callable, expression.Arguments, offset);
            
            return InvokeCallableObject(callable, arguments);
        }

        public Obj Execute(TernaryExpression expression)
        {
            var condition = EvaluateCondition(expression.Condition);
            
            return condition.Value
                ? expression.ThenExpression.Accept(this)
                : expression.ElseExpression.Accept(this);
        }

        public Obj Execute(AttributeAccessExpression expression)
        {
            var obj = expression.ObjExpression.Accept(this);
            var attributeName = expression.Attribute.Text;

            if (!obj.TryGetAttribute(attributeName, out var value))
            {
                throw new RuntimeException(
                    expression.ObjExpression.Location,
                    $"'{obj.TypeName}' object has not attribute '{attributeName}'");
            }

            return value;
        }

        public Obj Execute(AttributeAssignmentExpression assignment)
        {
            var accessExpression = assignment.AccessExpression;
            
            var obj = accessExpression.ObjExpression.Accept(this);
            var value = assignment.Value.Accept(this);
            
            obj.SetAttribute(accessExpression.Attribute.Text, value);

            return value;
        }

        private Obj InvokeCallableObject(Function callable, ImmutableArray<CallArgument> arguments)
        {
            callStack.Push(callable);

            var previousScope = scope;
            scope = new Scope(globalScope);
            
            foreach (var argument in arguments)
                scope.Assign(argument.Name, argument.Value);

            var returnedValue = callable.Invoke(new CallContext(callable, scope));

            scope = previousScope;
            callStack.Pop();

            return returnedValue;
        }

        private ImmutableArray<CallArgument> EvaluateArguments(
            Function callable, ImmutableArray<Expression> arguments, int positionsOffset)
        {
            var result = ImmutableArray.CreateBuilder<CallArgument>();

            var positions = callable
                .PositionParameters
                .Skip(positionsOffset)
                .ToImmutableArray();
            var defaults = callable.DefaultParameters;
            
            for (var i = 0; i < positions.Length; i++)
                result.Add(new CallArgument(positions[i], arguments[i].Accept(this)));

            for (var i = 0; i < arguments.Length - positions.Length; i++)
            {
                var value = arguments[i + positions.Length].Accept(this);
                result.Add(new CallArgument(defaults[i].Name, value));
            }

            for (var i = 0; i < positions.Length + defaults.Length - arguments.Length; i++)
            {
                var offset = i + arguments.Length - positions.Length;
                result.Add(defaults[offset]);
            }

            return result.ToImmutable();
        }

        private bool TryAssignUp(string name, Obj value)
        {
            var current = scope;
            do
            {
                if (current.Contains(name))
                {
                    current.Assign(name, value);
                    return true;
                }

                current = current.Parent;
            } while (current is not null);

            return false;
        }

        private void ExecuteLoopWithPreCondition<T>(
            T statement, 
            Action<T> initialize = null, 
            Action<T> updateAfterIteration = null) where T : LoopStatement
        {
            loopStack.Push(statement);

            try
            {
                initialize?.Invoke(statement);

                while (EvaluateCondition(statement.Condition).Value)
                {
                    try
                    {
                        statement.Body.Accept(this);
                    }
                    catch (BreakSignal)
                    {
                        break;
                    }
                    catch (ContinueSignal)
                    {
                    }

                    updateAfterIteration?.Invoke(statement);
                }
            }
            finally
            {
                loopStack.Pop();
            }
        }

        // TODO: implicit casting
        private Bool EvaluateCondition(Expression expression)
        {
            var obj = expression.Accept(this);

            if (obj is not Bool boolean)
                throw new RuntimeException(expression.Location, $"Cannot implicitly cast to '{nameof(Bool)}'");

            return boolean;
        }

        private static Location GetLocationBetweenBrackets(SyntaxToken open, SyntaxToken close)
        {
            return new Location(
                open.Location.SourceText,
                open.Location.Span.Start,
                close.Location.Span.Start - open.Location.Span.Start + 1
            );
        }
    }
}