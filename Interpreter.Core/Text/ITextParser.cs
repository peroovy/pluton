using System.Collections.Immutable;

namespace Interpreter.Core.Text
{
    public interface ITextParser
    {
        public ImmutableArray<Line> ParseLines(string text);
    }
}