using System.Collections.Immutable;
using Core.Execution;
using Core.Execution.Objects;
using Core.Lexing;
using Core.Syntax.AST.Expressions;

namespace Core.Syntax.AST
{
    public class ForStatement : Statement
    {
        public ForStatement(
            SyntaxToken keyword, 
            SyntaxToken openParenthesis, 
            ImmutableArray<Expression> initializers,
            SyntaxToken firstSemicolon,
            Expression condition,
            SyntaxToken secondSemicolon,
            ImmutableArray<Expression> iterators,
            SyntaxToken closeParenthesis,
            Statement body)
        {
            Keyword = keyword;
            OpenParenthesis = openParenthesis;
            Initializers = initializers;
            FirstSemicolon = firstSemicolon;
            Condition = condition;
            SecondSemicolon = secondSemicolon;
            Iterators = iterators;
            CloseParenthesis = closeParenthesis;
            Body = body;
        }
        
        public SyntaxToken Keyword { get; }
        
        public SyntaxToken OpenParenthesis { get; }
        
        public ImmutableArray<Expression> Initializers { get; }
        
        public SyntaxToken FirstSemicolon { get; }
        
        public Expression Condition { get; }
        
        public SyntaxToken SecondSemicolon { get; }
        
        public ImmutableArray<Expression> Iterators { get; }
        
        public SyntaxToken CloseParenthesis { get; }

        public Statement Body { get; }

        public override SyntaxToken FirstChild => Keyword;

        public override SyntaxToken LastChild => Body.LastChild;
        
        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}