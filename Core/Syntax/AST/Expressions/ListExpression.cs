using System.Collections.Immutable;
using Core.Execution;
using Core.Execution.Objects;
using Core.Lexing;

namespace Core.Syntax.AST.Expressions
{
    public class ListExpression : Expression
    {
        public ListExpression(SyntaxToken openBracket, ImmutableArray<Expression> items, SyntaxToken closeBracket)
        {
            OpenBracket = openBracket;
            Items = items;
            CloseBracket = closeBracket;
        }
        
        public SyntaxToken OpenBracket { get; }
        
        public ImmutableArray<Expression> Items { get; }
        
        public SyntaxToken CloseBracket { get; }

        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}