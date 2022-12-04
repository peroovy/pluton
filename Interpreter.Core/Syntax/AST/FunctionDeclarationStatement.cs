using System.Collections.Immutable;
using Interpreter.Core.Execution;
using Interpreter.Core.Execution.Objects;
using Interpreter.Core.Lexing;
using Interpreter.Core.Syntax.AST.Expressions;

namespace Interpreter.Core.Syntax.AST
{
    public class FunctionDeclarationStatement : Statement
    {
        public FunctionDeclarationStatement(
            SyntaxToken keyword, 
            SyntaxToken name, 
            SyntaxToken openParenthesis, 
            ImmutableArray<SyntaxToken> positionParameters,
            ImmutableArray<SyntaxDefaultParameter> defaultParameters,
            SyntaxToken closeParenthesis, 
            BlockStatement body)
        {
            Keyword = keyword;
            Name = name;
            OpenParenthesis = openParenthesis;
            PositionParameters = positionParameters;
            DefaultParameters = defaultParameters;
            CloseParenthesis = closeParenthesis;
            Body = body;
        }
        
        public SyntaxToken Keyword { get; }
        
        public SyntaxToken Name { get; }
        
        public SyntaxToken OpenParenthesis { get; }
        
        public ImmutableArray<SyntaxToken> PositionParameters { get; }
        
        public ImmutableArray<SyntaxDefaultParameter> DefaultParameters { get; }

        public SyntaxToken CloseParenthesis { get; }
        
        public BlockStatement Body { get; }
        
        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}