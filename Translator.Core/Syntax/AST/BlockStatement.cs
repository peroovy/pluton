using System.Collections.Immutable;
using Translator.Core.Execution;
using Translator.Core.Execution.Objects;
using Translator.Core.Lexing;

namespace Translator.Core.Syntax.AST
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