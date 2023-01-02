using System.Collections.Immutable;
using Core.Execution;
using Core.Execution.Objects;
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

        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}