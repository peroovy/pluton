using System.Collections.Immutable;
using Core.Execution;
using Core.Execution.DataModel.Objects;
using Core.Lexing;

namespace Core.Syntax.AST
{
    public class ClassStatement : Statement
    {
        public ClassStatement(
            SyntaxToken keyword,
            SyntaxToken identifier,
            SyntaxToken openBrace,
            ImmutableArray<SyntaxNode> members,
            SyntaxToken closeBrace)
        {
            Keyword = keyword;
            Identifier = identifier;
            OpenBrace = openBrace;
            Members = members;
            CloseBrace = closeBrace;
        }

        public override SyntaxToken FirstChild => Keyword;

        public override SyntaxToken LastChild => CloseBrace;
        
        public SyntaxToken Keyword { get; }
        
        public SyntaxToken Identifier { get; }
        
        public SyntaxToken OpenBrace { get; }
        
        public ImmutableArray<SyntaxNode> Members { get; }

        public SyntaxToken CloseBrace { get; }

        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}