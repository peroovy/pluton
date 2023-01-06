using System.Collections.Generic;
using System.Collections.Immutable;
using Core.Execution;
using Core.Execution.Objects;
using Core.Lexing;
using Core.Utils.Text;

namespace Core.Syntax.AST
{
    public class BlockStatement : Statement
    {
        public BlockStatement(
            SourceText sourceText, SyntaxToken openBrace, ImmutableArray<Statement> statements, SyntaxToken closeBrace)
            : base(sourceText)
        {
            OpenBrace = openBrace;
            Statements = statements;
            CloseBrace = closeBrace;
        }
        
        public SyntaxToken OpenBrace { get; }
        
        public ImmutableArray<Statement> Statements { get; }
        
        public SyntaxToken CloseBrace { get; }

        public override Obj Accept(IExecutor executor) => executor.Execute(this);
        
        public override IEnumerable<Location> GetChildrenLocations()
        {
            yield return OpenBrace.Location;

            foreach (var statement in Statements)
                yield return statement.Location;

            yield return CloseBrace.Location;
        }
    }
}