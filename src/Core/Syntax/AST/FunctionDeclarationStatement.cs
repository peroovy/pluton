using System.Collections.Immutable;
using Core.Execution;
using Core.Execution.DataModel.Objects;
using Core.Lexing;

namespace Core.Syntax.AST
{
    public class FunctionDeclarationStatement : Statement
    {
        public FunctionDeclarationStatement(
            SyntaxToken keyword, 
            SyntaxToken identifier, 
            SyntaxToken openParenthesis, 
            ImmutableArray<SyntaxToken> positionParameters,
            ImmutableArray<DefaultParameter> defaultParameters,
            SyntaxToken closeParenthesis, 
            BlockStatement block)
        {
            Keyword = keyword;
            Identifier = identifier;
            OpenParenthesis = openParenthesis;
            PositionParameters = positionParameters;
            DefaultParameters = defaultParameters;
            CloseParenthesis = closeParenthesis;
            Block = block;
        }
        
        public SyntaxToken Keyword { get; }
        
        public SyntaxToken Identifier { get; }
        
        public SyntaxToken OpenParenthesis { get; }
        
        public ImmutableArray<SyntaxToken> PositionParameters { get; }
        
        public ImmutableArray<DefaultParameter> DefaultParameters { get; }

        public SyntaxToken CloseParenthesis { get; }
        
        public BlockStatement Block { get; }

        public override SyntaxToken FirstChild => Keyword;

        public override SyntaxToken LastChild => Block.LastChild;
        
        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}