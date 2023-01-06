using System.Collections.Generic;
using System.Collections.Immutable;
using Core.Execution;
using Core.Execution.Objects;
using Core.Lexing;
using Core.Utils.Text;

namespace Core.Syntax.AST
{
    public class FunctionDeclarationStatement : Statement
    {
        public FunctionDeclarationStatement(
            SourceText sourceText,
            SyntaxToken keyword, 
            SyntaxToken identifier, 
            SyntaxToken openParenthesis, 
            ImmutableArray<SyntaxToken> positionParameters,
            ImmutableArray<DefaultParameter> defaultParameters,
            SyntaxToken closeParenthesis, 
            BlockStatement block) : base(sourceText)
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
        
        public override Obj Accept(IExecutor executor) => executor.Execute(this);
        
        public override IEnumerable<Location> GetChildrenLocations()
        {
            yield return Keyword.Location;
            yield return Identifier.Location;
            yield return OpenParenthesis.Location;

            foreach (var parameter in PositionParameters)
                yield return parameter.Location;

            foreach (var parameter in DefaultParameters)
                yield return parameter.Location;

            yield return CloseParenthesis.Location;
            yield return Block.Location;
        }
    }
}