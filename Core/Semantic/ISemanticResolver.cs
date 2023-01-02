using Translator.Core.Execution;
using Translator.Core.Execution.Objects;
using Translator.Core.Execution.Operations.Binary;
using Translator.Core.Logging;
using Translator.Core.Scoping;
using Translator.Core.Syntax.AST;
using Translator.Core.Syntax.AST.Expressions;

namespace Translator.Core.Semantic
{
    public abstract class SemanticNode
    {
        public abstract Obj Accept(IExecutor executor);
    }

    public abstract class SemanticStatement : SemanticNode
    {
        
    }

    public class SemanticExpressionStatement : SemanticStatement
    {
        public SemanticExpressionStatement(SemanticExpression expression)
        {
            Expression = expression;
        }
        
        public SemanticExpression Expression { get; }

        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }

    public abstract class SemanticExpression : SemanticNode
    {
    }

    public class SemanticNumber : SemanticExpression
    {
        public SemanticNumber(double value)
        {
            Value = value;
        }
        
        public double Value { get; }

        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
    
    public class SemanticBoolean : SemanticExpression
    {
        public SemanticBoolean(bool value)
        {
            Value = value;
        }
        
        public bool Value { get; }

        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }

    public class SemanticAssignmentExpression : SemanticExpression
    {
        public SemanticAssignmentExpression(string identifier, SemanticExpression expression)
        {
            Identifier = identifier;
            Expression = expression;
        }
        
        public string Identifier { get; }
        
        public SemanticExpression Expression { get; }

        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
    
    public class SemanticBinaryExpression : SemanticExpression
    {
        public SemanticBinaryExpression(SemanticExpression left, BinaryOperationMethod operation, SemanticExpression right)
        {
            Left = left;
            Operation = operation;
            Right = right;
        }
        
        public SemanticExpression Left { get; }
        
        public BinaryOperationMethod Operation { get; }
        
        public SemanticExpression Right { get; }
        
        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
    
    public class SemanticParenthesizedExpression : SemanticExpression
    {
        public SemanticParenthesizedExpression(SemanticExpression expression)
        {
            Expression = expression;
        }
        
        public SemanticExpression Expression { get; }

        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
    
    public class SemanticVariableExpression : SemanticExpression
    {
        public SemanticVariableExpression(string identifier)
        {
            Identifier = identifier;
        }
        
        public string Identifier { get; }

        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }

    public interface ISemanticResolver
    {
        SemanticNode Resolve(BlockStatement block);

        SemanticNode Resolve(IfStatement statement);

        SemanticNode Resolve(ElseClause clause);

        SemanticNode Resolve(ExpressionStatement statement);

        SemanticNode Resolve(AssignmentExpression expression);

        SemanticNode Resolve(BinaryExpression binary);

        SemanticNode Resolve(ParenthesizedExpression parenthesized);

        SemanticNode Resolve(BooleanExpression boolean);

        SemanticNode Resolve(NumberExpression number);

        SemanticNode Resolve(VariableExpression variable);
    }

    public class SemanticResolver : ISemanticResolver
    {
        private readonly ILogger logger;
        
        private Scope currentScope = new(null);
        
        public SemanticResolver(ILogger logger)
        {
            this.logger = logger;
        }

        public SemanticNode Resolve(BlockStatement block)
        {
            
        }

        public SemanticNode Resolve(IfStatement statement)
        {
            throw new System.NotImplementedException();
        }

        public SemanticNode Resolve(ElseClause clause)
        {
            
        }

        public SemanticNode Resolve(ExpressionStatement statement)
        {
            throw new System.NotImplementedException();
        }

        public SemanticNode Resolve(AssignmentExpression expression)
        {
            throw new System.NotImplementedException();
        }

        public SemanticNode Resolve(BinaryExpression binary)
        {
            var left = binary.Left.Accept(this);
            var right = binary.Left.Accept(this);
            
        }

        public SemanticNode Resolve(ParenthesizedExpression parenthesized)
        {
            var expression = (SemanticExpression)parenthesized.InnerExpression.Accept(this);

            return new SemanticParenthesizedExpression(expression);
        }

        public SemanticNode Resolve(BooleanExpression boolean) => new SemanticBoolean(boolean.Value);

        public SemanticNode Resolve(NumberExpression number) => new SemanticNumber(number.Value);

        public SemanticNode Resolve(VariableExpression variable)
        {
            if (currentScope.TryLookup(variable.Name.Text, out var value))
                return new SemanticVariableExpression(variable.Name.Text);

            var name = variable.Name;
            logger.Error(name.Location, name.Length, $"Variable '{name.Text}' does not exist");

            return null;
        }
    }
}