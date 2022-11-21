using System.Collections.Generic;
using System.Linq;
using Translator.Core.Execution.BinaryOperations;
using Translator.Core.Logging;
using Translator.Core.Syntax.AST;
using Translator.Core.Syntax.AST.Expressions;

namespace Translator.Core.Execution
{
    public class Executor : IExecutor
    {
        private readonly IBinaryOperation[] binaryOperations;
        private readonly ILogger logger;
        private readonly Dictionary<string, Object> variables = new Dictionary<string, Object>();

        public Executor(IBinaryOperation[] binaryOperations, ILogger logger)
        {
            this.binaryOperations = binaryOperations;
            this.logger = logger;
        }

        public Object Execute(ExpressionStatement statement) => statement.Expression.Accept(this);

        public Object Execute(AssignmentExpression assignment)
        {
            return variables[assignment.Variable.Value] = assignment.Expression.Accept(this);
        }

        public Object Execute(ParenthesizedExpression expression) => expression.InnerExpression.Accept(this);

        public Object Execute(BinaryExpression binary)
        {
            var op = binary.OperatorToken;
            
            var left = binary.Left.Accept(this);
            var right = binary.Right.Accept(this);
            var binaryOperation = binaryOperations.FirstOrDefault(operation =>
                operation.IsEvaluatedFor(left, binary.OperatorToken.Type, right)
            );

            if (binaryOperation != null) 
                return binaryOperation.Evaluate(left, right);
            
            logger.Error(op.Location, op.Length,
                $"The binary operator '{op.Value}' is not defined for '{left.Type.Name}' and '{right.Type.Name}' types");

            return new Object(null);
        }

        public Object Execute(NumberExpression number) => new Object(number.Value);

        public Object Execute(BooleanExpression boolean) => new Object(boolean.Value);
        
        public Object Execute(VariableExpression variable)
        {
            if (variables.TryGetValue(variable.Name.Value, out var value))
                return value;

            var nameToken = variable.Name;
            logger.Error(nameToken.Location, nameToken.Length,$"Undeclared variable '{nameToken.Value}'");

            return new Object(null);
        }
    }
}