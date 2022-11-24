using System.Collections.Immutable;
using Translator.Core.Execution;
using Translator.Core.Execution.Objects;
using Translator.Core.Lexing;
using Translator.Core.Syntax.AST.Expressions;

namespace Translator.Core.Syntax.AST
{
    public class ForStatement : Statement
    {
        public ForStatement(
            SyntaxToken keyword, 
            SyntaxToken openParenthesis, 
            ImmutableArray<Expression> initializers,
            SyntaxToken firstSectionSeparator,
            Expression condition,
            SyntaxToken secondSectionSeparator,
            ImmutableArray<Expression> iterators,
            SyntaxToken closeParenthesis,
            Statement body)
        {
            Keyword = keyword;
            OpenParenthesis = openParenthesis;
            Initializers = initializers;
            FirstSectionSeparator = firstSectionSeparator;
            Condition = condition;
            SecondSectionSeparator = secondSectionSeparator;
            Iterators = iterators;
            CloseParenthesis = closeParenthesis;
            Body = body;
        }
        
        public SyntaxToken Keyword { get; }
        
        public SyntaxToken OpenParenthesis { get; }
        
        public ImmutableArray<Expression> Initializers { get; }
        
        public SyntaxToken FirstSectionSeparator { get; }
        
        public Expression Condition { get; }
        
        public SyntaxToken SecondSectionSeparator { get; }
        
        public ImmutableArray<Expression> Iterators { get; }
        
        public SyntaxToken CloseParenthesis { get; }

        public Statement Body { get; }

        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}