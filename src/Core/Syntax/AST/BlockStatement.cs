using System.Collections.Immutable;
using Core.Execution;
using Core.Execution.DataModel.Objects;
using Core.Lexing;

namespace Core.Syntax.AST
{
    public class BlockStatement : Statement
    {
        public BlockStatement(SyntaxToken openBrace, ImmutableArray<Statement> statements, SyntaxToken closeBrace)
        {
            OpenBrace = openBrace;
            Statements = statements;
            CloseBrace = closeBrace;
        }
        
        public SyntaxToken OpenBrace { get; }
        
        public ImmutableArray<Statement> Statements { get; }
        
        public SyntaxToken CloseBrace { get; }

        public override SyntaxToken FirstChild => OpenBrace;

        public override SyntaxToken LastChild => CloseBrace;
        
        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}