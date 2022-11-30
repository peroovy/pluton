using System.Collections.Immutable;
using Interpreter.Core.Execution;
using Interpreter.Core.Execution.Objects;
using Interpreter.Core.Lexing;

namespace Interpreter.Core.Syntax.AST.Expressions
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