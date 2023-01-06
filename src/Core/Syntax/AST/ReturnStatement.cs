using System.Collections.Generic;
using Core.Execution;
using Core.Execution.Objects;
using Core.Lexing;
using Core.Syntax.AST.Expressions;
using Core.Utils.Text;

namespace Core.Syntax.AST
{
    public class ReturnStatement : Statement
    {
        public ReturnStatement(SourceText sourceText, SyntaxToken keyword, Expression expression, SyntaxToken semicolon)
            : base(sourceText)
        {
            Keyword = keyword;
            Expression = expression;
            Semicolon = semicolon;
        }
        
        public SyntaxToken Keyword { get; }
        
        public Expression Expression { get; }
        
        public SyntaxToken Semicolon { get; }

        public override SyntaxToken FirstChild => Keyword;

        public override SyntaxToken LastChild => Semicolon;
        
        public override Obj Accept(IExecutor executor) => executor.Execute(this);
    }
}