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

        public override SyntaxToken FirstChild => OpenBrace;

        public override SyntaxToken LastChild => CloseBrace;
        
        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}