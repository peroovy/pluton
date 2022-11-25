using System.Collections.Immutable;
using Interpreter.Core.Lexing;
using Interpreter.Core.Syntax.AST;

namespace Interpreter.Core.Syntax
{
    public interface ISyntaxParser
    {
        SyntaxNode Parse(ImmutableArray<SyntaxToken> tokens);
    }
}