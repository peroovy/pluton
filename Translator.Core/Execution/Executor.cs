using System.Linq;
using Translator.Core.Execution.Objects;
using Translator.Core.Execution.Operations.Binary;
using Translator.Core.Lexing;
using Translator.Core.Logging;
using Translator.Core.Syntax.AST;
using Translator.Core.Syntax.AST.Expressions;
using Boolean = Translator.Core.Execution.Objects.Boolean;

namespace Translator.Core.Execution
{
    public class Executor : IExecutor
    {
        private readonly BinaryOperation[] binaryOperations;
        private readonly ILogger logger;
        
        private Scope currentScope = new(null);

        public Executor(BinaryOperation[] binaryOperations, ILogger logger)
        {
            this.binaryOperations = binaryOperations;
            this.logger = logger;
        }

        public Obj Execute(WhileStatement statement)
        {
            while ((statement.Condition.Accept(this) as Boolean)?.IsTrue ?? false)
                statement.Body.Accept(this);

            return null;
        }

        public Obj Execute(IfStatement statement)
        {
            if (statement.Condition.Accept(this) is not Boolean boolean) 
                return null;
            
            if (boolean.IsTrue)
            {
                statement.Statement.Accept(this);
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

        public Obj Execute(Condition condition)
        {
            if (condition.Expression.Accept(this) is Boolean value) 
                return value;
            
            var open = condition.OpenParenthesis;
            var close = condition.CloseParenthesis;
            var location = new TextLocation(open.Location.Line, open.Location.Position + 1);
            logger.Error(location, close.Location.Position - open.Location.Position - 1,
                $"Type of the condition is not '{ObjectTypes.Boolean}'");

            return null;
        }

        public Obj Execute(BlockStatement block)
        {
            var parentScope = currentScope;
            currentScope = new Scope(parentScope);
            
            foreach (var statement in block.Statements)
                statement.Accept(this);

            currentScope = parentScope;

            return null;
        }

        public Obj Execute(ExpressionStatement statement)
        {
            // return null;
            // TODO: temp
            return statement.Expression.Accept(this);
        }

        public Obj Execute(AssignmentExpression assignment)
        {
            var name = assignment.Variable.Text;
            var value = assignment.Expression.Accept(this);

            if (value is Undefined)
                return value;

            var current = currentScope;
            do
            {
                if (current.Contains(name))
                    return current.Assign(name, value);

                current = current.Parent;
            } 
            while (current is not null);

            return this.currentScope.Assign(name, value);
        }

        public Obj Execute(ParenthesizedExpression expression) => expression.InnerExpression.Accept(this);

        public Obj Execute(BinaryExpression binary)
        {
            var opToken = binary.OperatorToken;
            
            var left = binary.Left.Accept(this);
            var right = binary.Right.Accept(this);
            var method = binaryOperations
                .First(op => op.IsOperator(opToken.Type))
                .GetMethod(left, right);

            if (method.IsUnknown)
            {
                logger.Error(opToken.Location, opToken.Length,
                    $"The binary operator '{opToken.Text}' is not defined for '{left.Type}' and '{right.Type}' types");
            }
            
            return method.Invoke(left, right);
        }

        public Obj Execute(NumberExpression number) => new Number(number.Value);

        public Obj Execute(BooleanExpression boolean) => new Boolean(boolean.Value);
        
        public Obj Execute(VariableExpression variable)
        {
            if (currentScope.TryLookup(variable.Name.Text, out var value))
                return value;

            var nameToken = variable.Name;
            logger.Error(nameToken.Location, nameToken.Length,$"Variable '{nameToken.Text}' does not exist");

            return new Undefined();
        }
    }
}